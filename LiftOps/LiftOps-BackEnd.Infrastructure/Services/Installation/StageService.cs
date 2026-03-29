using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Infrastructure.Persistence;
using LiftOps_BackEnd.Infrastructure.Repositories.Installation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Installation
{
    public class StageService : IStageService
    {
        private readonly IInstallationStageRepository _stageRepository;
        private readonly IStageRequiredPartRepository _requiredPartRepository;
        private readonly IStageTechnicianRepository _stageTechnicianRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IPdfGenerator _pdfGenerator; 
        private readonly ITechnicianService _technicianService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IGenericRepository<InventoryItem> _inventoryRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StageService> _logger;
        private readonly ICustomerStatusService _customerStatusService;

        public StageService(
            IInstallationStageRepository stageRepository,
            IStageRequiredPartRepository requiredPartRepository,
            IStageTechnicianRepository stageTechnicianRepository,
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IPdfGenerator pdfGenerator,
            ITechnicianService technicianService,
            IMaintenanceService maintenanceService,
            IGenericRepository<InventoryItem> inventoryRepository,
            INotificationService notificationService,
            ILogger<StageService> logger,
            ICustomerStatusService customerStatusService)
        {
            _stageRepository = stageRepository;
            _requiredPartRepository = requiredPartRepository;
            _stageTechnicianRepository = stageTechnicianRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _pdfGenerator = pdfGenerator;
            _technicianService = technicianService;
            _maintenanceService = maintenanceService;
            _inventoryRepository = inventoryRepository;
            _notificationService = notificationService;
            _logger = logger;
            _customerStatusService = customerStatusService;
        }

        public async Task StartStageAsync(Guid stageId, DateTime startDate)
        {
            var stage = await _stageRepository.GetByIdAsync(stageId);
            if (stage == null) throw new System.Exception("Stage not found");

            // Get elevator and project to check project status
            var elevator = await _unitOfWork.Repository<Elevator>().GetByIdAsync(stage.ElevatorId);
            if (elevator == null) throw new System.Exception("Elevator not found");
            
            // Load project with customer and projects included to check customer status
            var project = await _context.InstallationProjects
                .Include(p => p.Customer)
                .Include(p => p.Elevators)
                    .ThenInclude(e => e.Stages)
                .FirstOrDefaultAsync(p => p.Id == elevator.ProjectId);
            if (project == null) throw new System.Exception("Project not found");

            // Calculate customer status dynamically based on all their projects
            var calculatedCustomerStatus = await _customerStatusService.GetCalculatedCustomerStatusAsync(project.CustomerId);

            // Check both project and customer status, provide detailed error message
            var rejectionReasons = new List<string>();
            
            if (project.ProjectStatus == ProjectStatus.Rejected)
            {
                rejectionReasons.Add("Project status is Rejected");
            }

            if (calculatedCustomerStatus == CustomerStatus.Rejected)
            {
                rejectionReasons.Add($"Client '{project.Customer.Name}' status is Rejected (calculated from projects)");
            }

            if (rejectionReasons.Any())
            {
                var reasonDetails = string.Join(" and ", rejectionReasons);
                throw new System.Exception($"Cannot start stage. {reasonDetails}. Please contact the administrator to resolve this issue. You may need to approve the project inspection or update the client status.");
            }

            // Prevent starting stages if project is pending inspection approval
            if (project.ProjectStatus == ProjectStatus.UnderInspectionAndQuotation)
            {
                throw new System.Exception("Cannot start stage. Project inspection must be approved first.");
            }

            // Enforcement of sequentiality: Check if previous stage is success
            if (stage.StageNumber > 1)
            {
                var prevStage = await FindStageByNumberAsync(stage.ElevatorId, stage.StageNumber - 1);
                if (prevStage == null || prevStage.Status != StageStatus.Success)
                {
                    throw new System.Exception($"Cannot start Stage {stage.StageNumber} before Stage {stage.StageNumber - 1} is completed.");
                }
            }
            
            stage.Status = StageStatus.InProgress;
            stage.StartDate = startDate;
            
            _stageRepository.Update(stage);
            await _unitOfWork.Complete();
        }

        public async Task UpdateStageAsync(Guid stageId, List<PartSelectionDto>? parts, string? notes, decimal? supplyCost, decimal? stagePrice, DateTime? endDate, List<Guid>? technicianIds)
        {
            var stage = await _stageRepository.GetStageWithPartsAsync(stageId);
            if (stage == null) throw new System.Exception("Stage not found");

            // Allow updating stages that are InProgress or Pending
            // Completed (Success) stages cannot be updated
            if (stage.Status == StageStatus.Success)
            {
                throw new System.Exception("Cannot update completed stages. Only pending or in-progress stages can be updated.");
            }
            
            // If stage is Pending, only allow updating stagePrice (for price distribution)
            // Other fields (parts, notes, etc.) can only be updated when stage is InProgress
            if (stage.Status == StageStatus.Pending)
            {
                // Only allow updating stagePrice for pending stages
                // Other updates require stage to be InProgress
                if (parts != null || notes != null || supplyCost.HasValue || endDate.HasValue || technicianIds != null)
                {
                    throw new System.Exception("Cannot update parts, notes, supply cost, end date, or technicians for pending stages. Start the stage first.");
                }
            }

            // Update notes if provided
            if (notes != null)
            {
                stage.Notes = notes;
            }

            // Update supply cost if provided
            if (supplyCost.HasValue)
            {
                stage.SupplyCost = supplyCost.Value;
            }

            // Update stage price if provided
            if (stagePrice.HasValue)
            {
                stage.StagePrice = stagePrice.Value;
            }

            // Update end date if provided
            if (endDate.HasValue)
            {
                stage.EndDate = endDate.Value;
            }

            // Update technicians if provided
            if (technicianIds != null)
            {
                // Delete existing technician assignments
                var existingTechnicians = await _stageTechnicianRepository.GetByStageIdAsync(stageId);
                foreach (var existing in existingTechnicians)
                {
                    _stageTechnicianRepository.Delete(existing);
                }

                // Add new technician assignments
                foreach (var techId in technicianIds)
                {
                    var stageTechnician = new StageTechnician
                    {
                        StageId = stageId,
                        TechnicianId = techId
                    };
                    _stageTechnicianRepository.Add(stageTechnician);
                }
            }

            // Replace parts if provided
            if (parts != null)
            {
                // Delete existing parts
                var existingParts = await _context.StageRequiredParts
                    .Where(p => p.StageId == stageId)
                    .ToListAsync();
                
                foreach (var existingPart in existingParts)
                {
                    _requiredPartRepository.Delete(existingPart);
                }

                // Add new parts
                foreach (var partDto in parts)
                {
                    var item = await _inventoryRepository.GetByIdAsync(partDto.InventoryItemId);
                    if (item == null) continue;

                    bool isOutOfStock = item.StockQuantity < partDto.Quantity;

                    var requiredPart = new StageRequiredPart
                    {
                        StageId = stageId,
                        InventoryItemId = partDto.InventoryItemId,
                        Quantity = partDto.Quantity,
                        IsOutOfStock = isOutOfStock
                    };

                    _requiredPartRepository.Add(requiredPart);

                    if (isOutOfStock)
                    {
                        await _notificationService.CreateNotificationAsync(
                            $"Part {item.Name} is Out of Stock for Stage {stageId}",
                            Guid.Empty // Pending: Resolve Admin ID
                        );
                    }
                }
            }

            _stageRepository.Update(stage);
            await _unitOfWork.Complete();
        }

        public async Task CompleteStageAsync(Guid stageId, decimal? supplyCost, string? notes, DateTime? endDate, decimal? price, bool collectPrice, int? freeMonths, List<TechnicianRatingDto>? technicianRatings)
        {
            var stage = await _stageRepository.GetStageWithPartsAsync(stageId);
            if (stage == null) throw new System.Exception("Stage not found");

            // Get project to validate prices
            var project = stage.Elevator?.Project;
            if (project == null)
            {
                // Load project if not included
                var elevator = await _unitOfWork.Repository<Elevator>().GetByIdAsync(stage.ElevatorId);
                if (elevator == null) throw new System.Exception("Elevator not found");
                project = await _context.InstallationProjects
                    .Include(p => p.Customer)
                    .Include(p => p.Elevators)
                        .ThenInclude(e => e.Stages)
                    .FirstOrDefaultAsync(p => p.Id == elevator.ProjectId);
                if (project == null) throw new System.Exception("Project not found");
            }
            else
            {
                // Ensure Customer and projects are loaded for status calculation
                if (project.Customer == null)
                {
                    await _context.Entry(project).Reference(p => p.Customer).LoadAsync();
                }
                // Load elevators and stages if not loaded
                if (project.Elevators == null || !project.Elevators.Any())
                {
                    await _context.Entry(project).Collection(p => p.Elevators).LoadAsync();
                    foreach (var elev in project.Elevators)
                    {
                        await _context.Entry(elev).Collection(e => e.Stages).LoadAsync();
                    }
                }
            }

            // Prevent completing stages if project is pending inspection approval
            if (project.ProjectStatus == ProjectStatus.UnderInspectionAndQuotation)
            {
                throw new System.Exception("Cannot complete stage. Project inspection must be approved first.");
            }

            // Calculate customer status dynamically based on all their projects
            var calculatedCustomerStatus = await _customerStatusService.GetCalculatedCustomerStatusAsync(project.CustomerId);

            // Check both project and customer status, provide detailed error message
            var rejectionReasons = new List<string>();
            
            if (project.ProjectStatus == ProjectStatus.Rejected)
            {
                rejectionReasons.Add("Project status is Rejected");
            }

            // Only check for Rejected customer status - PendingInspectionQuotation is allowed (project will be approved)
            if (calculatedCustomerStatus == CustomerStatus.Rejected)
            {
                rejectionReasons.Add($"Client '{project.Customer.Name}' status is Rejected (calculated from projects)");
            }

            if (rejectionReasons.Any())
            {
                var reasonDetails = string.Join(" and ", rejectionReasons);
                throw new System.Exception($"Cannot complete stage. {reasonDetails}. Please contact the administrator to resolve this issue. You may need to approve the project inspection or update the client status.");
            }

            // Validate that previous stage price is paid before completing current stage
            if (stage.StageNumber > 1)
            {
                var prevStage = await FindStageByNumberAsync(stage.ElevatorId, stage.StageNumber - 1);
                if (prevStage != null && prevStage.Status == StageStatus.Success)
                {
                    // Previous stage must have price collected
                    if (!prevStage.IsPriceCollected)
                    {
                        throw new System.Exception($"Cannot complete Stage {stage.StageNumber}. Stage {stage.StageNumber - 1} price must be paid first.");
                    }
                }
            }

            // Validate that price is provided (can be 0) before completing stage
            if (!price.HasValue || price.Value < 0)
            {
                throw new System.Exception("Stage price is required and must be 0 or greater");
            }

            if (!collectPrice)
            {
                throw new System.Exception("Cannot complete stage. Price must be collected before completing the stage.");
            }

            // Note: Price validation against elevator total is now handled at the frontend level.
            // The backend no longer validates against project.TotalPrice since we use per-elevator pricing
            // where each elevator's stage prices sum to that elevator's total (not the project total).

            // Generate PDF first (before modifying stage)
            string? pdfPath = null;
            try 
            {
                pdfPath = await _pdfGenerator.GenerateStageReportAsync(stage);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"PDF Generation Failed: {ex.Message}");
            }

            // Store values before detaching
            var stageNumber = stage.StageNumber;
            var elevatorId = stage.ElevatorId;
            
            _logger.LogInformation($"Completing stage {stageId}. Stage Number: {stageNumber}, Elevator ID: {elevatorId}");
            
            // Detach ALL related entities to avoid any tracking conflicts
            _context.Entry(stage).State = EntityState.Detached;
            if (stage.Elevator != null)
            {
                _context.Entry(stage.Elevator).State = EntityState.Detached;
            }
            if (stage.Elevator?.Project != null)
            {
                _context.Entry(stage.Elevator.Project).State = EntityState.Detached;
            }
            if (stage.Elevator?.Project?.Customer != null)
            {
                _context.Entry(stage.Elevator.Project.Customer).State = EntityState.Detached;
            }
            _logger.LogInformation($"Detached all related entities");

            // Save any pending changes from UpdateStage call first (before we do direct updates)
            try
            {
                var savedChanges = await _unitOfWork.Complete();
                _logger.LogInformation($"Saved pending changes from UpdateStage call. Affected rows: {savedChanges}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, $"Concurrency exception when saving pending changes (this is OK, we'll update directly): {ex.Message}");
                // Detach any conflicting entities
                foreach (var entry in ex.Entries)
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error saving pending changes (continuing with direct update): {ex.Message}");
                // Continue - we'll update directly anyway
            }

            // Use direct database update to avoid entity tracking conflicts
            // This bypasses EF Core's change tracking and updates the database directly
            var endDateValue = endDate ?? DateTime.UtcNow;
            
            _logger.LogInformation($"Updating stage {stageId} directly in database. Status: Success, Price: {price}, EndDate: {endDateValue}");

            // Use ExecuteUpdateAsync to update directly without entity tracking
            // This executes a SQL UPDATE statement directly, bypassing change tracking
            var updateResult = await _context.InstallationStages
                .Where(s => s.Id == stageId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Status, StageStatus.Success)
                    .SetProperty(s => s.EndDate, endDateValue)
                    .SetProperty(s => s.SupplyCost, supplyCost)
                    .SetProperty(s => s.StagePrice, price)
                    .SetProperty(s => s.IsPriceCollected, collectPrice)
                    .SetProperty(s => s.Notes, notes)
                    .SetProperty(s => s.PdfPath, s => !string.IsNullOrEmpty(pdfPath) ? pdfPath : s.PdfPath)
                    .SetProperty(s => s.LastModifiedAt, DateTime.UtcNow));

            _logger.LogInformation($"Direct update completed. Affected rows: {updateResult}");

            if (updateResult == 0)
            {
                _logger.LogError($"Stage {stageId} not found or could not be updated. The stage may have been deleted.");
                throw new System.Exception($"Stage {stageId} not found or could not be updated. Please refresh and try again.");
            }

            // Validate and save technician ratings
            // Get assigned technicians - if none exist, ratings are not required
            var assignedTechnicians = await _stageTechnicianRepository.GetByStageIdAsync(stageId);
            
            // If technicians are assigned, ratings are required
            if (assignedTechnicians != null && assignedTechnicians.Count > 0)
            {
                // Validate that ratings are provided for all assigned technicians
                if (technicianRatings == null || technicianRatings.Count == 0)
                {
                    throw new System.Exception("Technician ratings are required. Please rate all assigned technicians.");
                }

                var assignedTechIds = assignedTechnicians.Select(st => st.TechnicianId).ToList();
                var ratedTechIds = technicianRatings.Select(r => r.TechnicianId).ToList();

                // Check that all assigned technicians have ratings
                var missingRatings = assignedTechIds.Where(id => !ratedTechIds.Contains(id)).ToList();
                if (missingRatings.Any())
                {
                    throw new System.Exception($"Ratings are required for all assigned technicians. Missing ratings for {missingRatings.Count} technician(s).");
                }

                // Validate rating values (typically 1-5)
                foreach (var rating in technicianRatings)
                {
                    if (rating.Rating < 1 || rating.Rating > 5)
                    {
                        throw new System.Exception($"Rating must be between 1 and 5. Invalid rating: {rating.Rating}");
                    }
                }

                // Update ratings for each technician using direct database update
                foreach (var ratingDto in technicianRatings)
                {
                    var stageTechnician = assignedTechnicians.FirstOrDefault(st => st.TechnicianId == ratingDto.TechnicianId);
                    if (stageTechnician != null)
                    {
                        // Use ExecuteUpdateAsync to update directly without tracking
                        await _context.Set<StageTechnician>()
                            .Where(st => st.Id == stageTechnician.Id)
                            .ExecuteUpdateAsync(setters => setters
                                .SetProperty(st => st.Rating, ratingDto.Rating)
                                .SetProperty(st => st.LastModifiedAt, DateTime.UtcNow));
                    }
                }
            }

            // No need to call _unitOfWork.Complete() here since we used ExecuteUpdateAsync
            // which executes directly against the database without going through the change tracker
            _logger.LogInformation($"Stage {stageId} completion successful. All updates were done via direct database updates.");

            // Update overall rating for each technician AFTER saving the stage ratings
            // Note: UpdateTechnicianOverallRatings uses direct database updates, so no need to call Complete()
            if (assignedTechnicians != null && assignedTechnicians.Count > 0 && technicianRatings != null && technicianRatings.Count > 0)
            {
                try
                {
                    await UpdateTechnicianOverallRatings(technicianRatings.Select(r => r.TechnicianId).ToList());
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to update technician overall ratings: {ex.Message}. Continuing...");
                    // Don't fail the stage completion if rating update fails
                }
            }

            // Trigger next stage automatically (only if project and customer are not rejected)
            if (stageNumber < 4 && 
                project.ProjectStatus != ProjectStatus.Rejected && 
                project.Customer.Status != CustomerStatus.Rejected)
            {
                // Auto-start next stage using direct update to avoid tracking conflicts
                var nextStageId = await _context.InstallationStages
                    .Where(s => s.ElevatorId == elevatorId && s.StageNumber == stageNumber + 1)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();
                
                if (nextStageId != Guid.Empty)
                {
                    // Use direct update to avoid tracking conflicts
                    await _context.InstallationStages
                        .Where(s => s.Id == nextStageId && s.Status == StageStatus.Pending)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(s => s.Status, StageStatus.InProgress)
                            .SetProperty(s => s.StartDate, DateTime.UtcNow)
                            .SetProperty(s => s.LastModifiedAt, DateTime.UtcNow));
                    
                    _logger.LogInformation($"Auto-started next stage {stageNumber + 1} for elevator {elevatorId}");
                }
            }
            else
            {
                // Stage 4 completed for this elevator
                // 1. Update Technician Stats
                try
                {
                    await _technicianService.UpdateTechnicianStatsAsync(stage.ElevatorId);
                }
                catch (System.Exception ex)
                {
                    _logger.LogWarning(ex, $"Stats Update Failed: {ex.Message}");
                }

                // 2. Check if all elevators in project have all stages completed
                // If all complete, transfer entire project to maintenance (handled in CheckAndMarkProjectAsCompleteAsync)
                // Pass free months from stage completion
                int freeMonthsValue = freeMonths ?? 6; // Default 6 months
                await CheckAndMarkProjectAsCompleteAsync(project.Id, freeMonthsValue);
            }
        }

        private async Task<InstallationStage?> FindStageByNumberAsync(Guid elevatorId, int stageNumber)
        {
            var stages = await _stageRepository.ListAllAsync();
            return stages.FirstOrDefault(s => s.ElevatorId == elevatorId && s.StageNumber == stageNumber);
        }

        private async Task CheckAndMarkProjectAsCompleteAsync(Guid projectId, int defaultFreeMonths = 6)
        {
            var project = await _unitOfWork.Repository<InstallationProject>().GetByIdAsync(projectId);
            if (project == null) return;

            // Get all stages for all elevators in this project
            var allStages = await _stageRepository.ListAllAsync();
            
            // Get all elevators for this project
            var allElevators = await _unitOfWork.Repository<Elevator>().ListAllAsync();
            var projectElevators = allElevators.Where(e => e.ProjectId == projectId).ToList();

            if (projectElevators.Count == 0) return;

            // Check if all elevators have all 4 stages completed
            bool allElevatorsComplete = true;
            int totalFreeMonths = 0;
            int completedElevatorsCount = 0;
            
            foreach (var elevator in projectElevators)
            {
                var elevatorStages = allStages
                    .Where(s => s.ElevatorId == elevator.Id)
                    .ToList();

                // Must have exactly 4 stages and all must be Success
                if (elevatorStages.Count != 4 || 
                    elevatorStages.Any(s => s.Status != StageStatus.Success))
                {
                    allElevatorsComplete = false;
                    break;
                }
                
                // Count completed elevators (all stages are Success)
                completedElevatorsCount++;
            }

            // If all elevators are complete, mark project as complete and transfer to maintenance
            if (allElevatorsComplete)
            {
                if (!project.ExpectedFinishDate.HasValue)
                {
                    project.ExpectedFinishDate = DateTime.UtcNow;
                }
                
                // Use the default free months for all elevators in the project
                // All elevators in the same project get the same free months
                int freeMonthsPerElevator = defaultFreeMonths;
                
                // Transfer all elevators in the project to maintenance
                // Use the same free months for all elevators in the project
                foreach (var elevator in projectElevators)
                {
                    try
                    {
                        await _maintenanceService.TransferElevatorToMaintenanceAsync(elevator.Id, freeMonthsPerElevator);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to transfer elevator {elevator.Id} to maintenance: {ex.Message}");
                        // Continue with other elevators even if one fails
                    }
                }
                
                // Entity is already tracked, just save
                await _unitOfWork.Complete();
                
                _logger.LogInformation($"Project {projectId} completed. All {projectElevators.Count} elevators transferred to maintenance with {freeMonthsPerElevator} free months each.");
            }
        }

        private async Task UpdateTechnicianOverallRatings(List<Guid> technicianIds)
        {
            foreach (var techId in technicianIds)
            {
                // Get all ratings for this technician from completed stages
                var ratingValues = await _context.Set<StageTechnician>()
                    .Where(st => st.TechnicianId == techId && st.Rating.HasValue)
                    .Select(st => st.Rating!.Value)
                    .ToListAsync();

                if (ratingValues.Any())
                {
                    var averageRating = ratingValues.Average();
                    var roundedRating = Math.Round(averageRating, 2);
                    
                    // Use direct SQL update to ensure the rating is saved
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Technicians SET OverallRating = {0} WHERE Id = {1}",
                        roundedRating,
                        techId);
                }
                else
                {
                    // If no ratings, set to NULL
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Technicians SET OverallRating = NULL WHERE Id = {0}",
                        techId);
                }
            }
        }
    }
}
