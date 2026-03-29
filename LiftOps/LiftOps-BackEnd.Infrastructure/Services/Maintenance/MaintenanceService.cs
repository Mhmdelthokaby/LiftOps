using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Installation; // Corrected
using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces; // IUnitOfWork
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LiftOps_BackEnd.Infrastructure.Services.Maintenance
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IGenericRepository<LiftOps_BackEnd.Domain.Entities.InventoryItem> _inventoryRepo;

        public MaintenanceService(
            IMaintenanceRepository repository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
             // Hack: using UoW to get generic repo for InventoryItem since we didn't inject specialized one here yet
             _inventoryRepo = unitOfWork.Repository<LiftOps_BackEnd.Domain.Entities.InventoryItem>();
        }

        public async Task<MaintenanceContract> CreateContractAsync(MaintenanceContract contract)
        {
            // IMPORTANT: When creating a maintenance contract, convert the customer status to Approved (Active)
            // This ensures that customers with maintenance contracts are always in Approved status,
            // regardless of their previous status (e.g., Rejected, PendingInspectionQuotation)
            // Rule: If client was rejected or pending, and we add a new maintenance project for them, they must be Approved
            var customerRepo = _unitOfWork.Repository<Customer>();
            var customer = await customerRepo.GetByIdAsync(contract.CustomerId);
            if (customer == null)
            {
                throw new Exception($"Customer with ID {contract.CustomerId} not found.");
            }
            
            // Set customer status to Approved (Active) when maintenance contract is created
            customer.Status = CustomerStatus.Approved;
            customerRepo.Update(customer);
            
            _repository.Add(contract);
            await _unitOfWork.Complete();
            return contract;
        }

        public async Task UpdateContractAsync(Guid contractId, UpdateMaintenanceContractDto dto)
        {
            var contract = await _repository.GetByIdAsync(contractId);
            if (contract == null)
                throw new Exception("Contract not found");

            // Validate project number uniqueness if it's being changed
            if (!string.IsNullOrWhiteSpace(dto.ProjectNumber) && dto.ProjectNumber != contract.ProjectNumber)
            {
                if (await ProjectNumberExistsAsync(dto.ProjectNumber))
                {
                    throw new Exception($"Project number '{dto.ProjectNumber}' already exists. Please use a different project number.");
                }
            }

            // Update contract properties
            contract.ProjectNumber = dto.ProjectNumber;
            contract.ProjectAddress = dto.ProjectAddress;
            // Always update city when provided in DTO
            // Frontend always sends city field (empty string if cleared, or the actual city value)
            if (dto.City != null)
            {
                var trimmedCity = dto.City.Trim();
                // If empty or whitespace, set to default; otherwise use the provided value
                contract.City = string.IsNullOrWhiteSpace(trimmedCity) ? "القاهرة الجديدة" : trimmedCity;
            }
            // Note: If dto.City is null (not provided), we keep the existing contract.City value
            contract.GoogleMapsLink = dto.GoogleMapsLink;
            contract.StartDate = dto.StartDate;
            contract.EndDate = dto.EndDate;
            contract.PricePerMonth = dto.PricePerMonth;
            contract.FreeMonths = dto.FreeMonths;
            contract.TechnicianId = dto.TechnicianId;

            _repository.Update(contract);

            // If this contract was created from an installation project, update the related InstallationProject
            if (contract.IsFromInstallation && !string.IsNullOrWhiteSpace(contract.ProjectNumber))
            {
                var installationProjectRepo = _unitOfWork.Repository<InstallationProject>();
                var installationProjects = await installationProjectRepo.ListAllAsync();
                var relatedProject = installationProjects.FirstOrDefault(p => p.ProjectNumber == contract.ProjectNumber);
                
                if (relatedProject != null)
                {
                    // Update InstallationProject with the same fields from MaintenanceContract
                    relatedProject.City = contract.City;
                    relatedProject.ProjectAddress = contract.ProjectAddress;
                    relatedProject.GoogleMapsLink = contract.GoogleMapsLink;
                    
                    installationProjectRepo.Update(relatedProject);
                }
            }

            await _unitOfWork.Complete();
        }

        public async Task<MaintenanceContract> AddElevatorToContractAsync(Guid contractId, MaintenanceElevator elevator)
        {
            var contract = await _repository.GetByIdAsync(contractId);
            if (contract == null) throw new Exception("Contract not found");

            elevator.ContractId = contractId;
            
            // Add elevator directly using UnitOfWork generic repository
            // This ensures the elevator is added as a new entity, not updated
            var elevatorRepo = _unitOfWork.Repository<MaintenanceElevator>();
            elevatorRepo.Add(elevator);
            await _unitOfWork.Complete();
            
            return contract;
        }

        public async Task UpdateElevatorAsync(Guid elevatorId, UpdateMaintenanceElevatorDto dto)
        {
            var elevator = await _repository.GetElevatorByIdAsync(elevatorId);
            if (elevator == null)
                throw new Exception("Elevator not found");

            // Update elevator properties
            elevator.Type = dto.Type;
            elevator.NumberOfStops = dto.NumberOfStops;
            elevator.NumberOfFloors = dto.NumberOfFloors;
            elevator.NextMaintenanceDate = dto.NextMaintenanceDate;

            await _repository.UpdateElevatorAsync(elevator);
            await _unitOfWork.Complete();
        }

        public async Task<IReadOnlyList<MaintenanceContract>> GetCustomerContractsAsync(Guid customerId)
        {
            return await _repository.GetContractsByCustomerAsync(customerId);
        }

        public async Task GenerateMonthlyVisitsAsync(int month, int year)
        {
            // Logic: Find all active contracts/elevators.
            // Check if visit already exists for this month. 
            // If not, create 'Pending' visit.
            
            // Simplified: Iterate all contracts (BAD for performance if millions, but OK for now).
            // A better way is a query: GetActiveElevators needing maintenance.
            
            // For MVP:
            // 1. Get all contracts. 
            // 2. Filter Elevators.
            // 3. Create visits.
            // This requires a new method in Repository: GetActiveElevatorsAsync().
            // I'll skip implementing that strictly and just use GetContracts for iteration.
            
             // Note: In real world, use a batch job or specific optimized query.
             var contracts = await _repository.ListAllAsync(); // Potentially heavy
             foreach(var c in contracts)
             {
                 if(c.Status != MaintenanceContractStatus.Active) continue;
                 // Need to fetch elevators if not lazy loaded. 
                 // ListAllAsync might not include them. 
                 // Better: _repository.GetActiveElevatorsToScheduleAsync() -> TODO
                 
                 // Since I didn't add that method, I'll implement 'ScheduleVisitAsync' individually 
                 // and assume the caller (Controller/worker) handles the loop or I iterate simplistic way.
             }
             
             // Placeholder for batch generation logic.
             await Task.CompletedTask;
        }

        public async Task<MaintenanceVisit> ScheduleVisitAsync(Guid elevatorId, DateTime date, Guid? technicianId = null)
        {
            // Validate that the visit date is not in the future
            var today = DateTime.UtcNow.Date;
            var visitDate = date.Date;
            
            if (visitDate > today)
            {
                throw new InvalidOperationException($"Cannot schedule maintenance visit for future dates. Today is {today:yyyy-MM-dd}, but visit date is {visitDate:yyyy-MM-dd}.");
            }

            // Validate that maintenance can only be done once per month
            var month = visitDate.Month;
            var year = visitDate.Year;
            var hasCompletedVisit = await _repository.HasCompletedVisitForMonthAsync(elevatorId, month, year);
            
            if (hasCompletedVisit)
            {
                var monthName = visitDate.ToString("MMMM yyyy");
                throw new InvalidOperationException($"Maintenance has already been completed for this elevator in {monthName}. Only one maintenance visit is allowed per month.");
            }
            
            var visit = new MaintenanceVisit
            {
                MaintenanceElevatorId = elevatorId,
                VisitDate = date,
                TechnicianId = technicianId,
                Status = VisitStatus.Pending
            };

            // Default to the project's technician if not provided
            if (!visit.TechnicianId.HasValue)
            {
                var elevator = await _repository.GetElevatorByIdAsync(elevatorId);
                if (elevator != null)
                {
                    var contract = await _repository.GetByIdAsync(elevator.ContractId);
                    if (contract != null)
                    {
                        visit.TechnicianId = contract.TechnicianId;
                    }
                }
            }
            
            await _repository.AddVisitAsync(visit);
            await _unitOfWork.Complete();
            return visit;
        }

        public async Task AssignTechnicianToVisitAsync(Guid visitId, Guid? technicianId, string? notes = null)
        {
            var visit = await _repository.GetVisitByIdAsync(visitId);
            if(visit == null) throw new Exception("Visit not found");
            
            visit.TechnicianId = technicianId;
            
            // Overwrite notes if provided
            visit.Notes = notes;
            
            // visit.Status = VisitStatus.InProgress; // Or keep Pending until they actually start?
            
            await _repository.UpdateVisitAsync(visit);
            await _unitOfWork.Complete();
        }

        public async Task UpdateVisitAsync(MaintenanceVisit visit)
        {
            await _repository.UpdateVisitAsync(visit);
            await _unitOfWork.Complete();
        }

        public async Task CompleteVisitAsync(Guid visitId, string notes, List<(Guid ItemId, int Qty)> partsUsed, List<(Guid ChecklistItemId, bool IsCompleted, string? Notes, int? Count, decimal? Percentage)>? checklistItems = null, string? paymentNotes = null)
        {
             var visit = await _repository.GetVisitByIdAsync(visitId);
            if(visit == null) throw new Exception("Visit not found");

            // Validate that maintenance can only be completed once per month for this elevator
            // Skip validation if visit is already completed (allows updating existing completed visit)
            if (visit.Status != VisitStatus.Done)
            {
                var visitDate = visit.VisitDate.Date;
                var month = visitDate.Month;
                var year = visitDate.Year;
                var hasCompletedVisit = await _repository.HasCompletedVisitForMonthAsync(visit.MaintenanceElevatorId, month, year);
                
                if (hasCompletedVisit)
                {
                    var monthName = visitDate.ToString("MMMM yyyy");
                    throw new InvalidOperationException($"Maintenance has already been completed for this elevator in {monthName}. Only one maintenance visit is allowed per month.");
                }
            }
            
            visit.Status = VisitStatus.Done;
            visit.CompletedDate = DateTime.UtcNow;
            visit.Notes = notes;
            visit.PaymentNotes = paymentNotes;
            
            foreach(var part in partsUsed)
            {
                var item = await _inventoryRepo.GetByIdAsync(part.ItemId);
                if(item == null) continue;

                // Create usage
                var usage = new MaintenanceSparePartUsage
                {
                    MaintenanceVisitId = visitId,
                    InventoryItemId = part.ItemId,
                    Quantity = part.Qty,
                    PriceAtTimeOfUsage = item.UnitPrice,
                    IsPaid = false // Default
                };
                visit.SpareParts.Add(usage);
                
                // Inventory Logic
                if (item.StockQuantity < part.Qty)
                {
                    // Create Notification
                    await _notificationService.CreateNotificationAsync(new Domain.Entities.Installation.Notification
                    {
                        Title = "Low/No Stock Used in Maintenance",
                        Message = $"Item {item.Name} used in Visit {visitId} but stock was insufficient.",
                        Type = Domain.Entities.Installation.NotificationType.OutOfStock,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow,
                        // UserId = ??? (Inventory Admin)
                    });
                }
                
                // Deduct stock regardless? Or allow negative? Requirements say "Allow selection even if stock=0".
                item.StockQuantity -= part.Qty;
                _inventoryRepo.Update(item);
            }

            // Handle checklist items
            var visitChecklistItemRepo = _unitOfWork.Repository<MaintenanceVisitChecklistItem>();
            
            // If updating an existing visit, remove old checklist items first
            if (visit.Status == VisitStatus.Done && visit.ChecklistItems != null && visit.ChecklistItems.Any())
            {
                foreach (var existingItem in visit.ChecklistItems.ToList())
                {
                    visitChecklistItemRepo.Delete(existingItem);
                }
            }
            
            // Add/update checklist items
            if (checklistItems != null && checklistItems.Any())
            {
                var checklistItemRepo = _unitOfWork.Repository<MaintenanceChecklistItem>();
                
                foreach (var checklistItem in checklistItems)
                {
                    // Verify checklist item exists and is active
                    var item = await checklistItemRepo.GetByIdAsync(checklistItem.ChecklistItemId);
                    if (item == null || !item.IsActive) continue;

                    // Create visit checklist item
                    var visitChecklistItem = new MaintenanceVisitChecklistItem
                    {
                        VisitId = visitId,
                        ChecklistItemId = checklistItem.ChecklistItemId,
                        IsCompleted = checklistItem.IsCompleted,
                        Notes = checklistItem.Notes,
                        Count = checklistItem.Count,
                        Percentage = checklistItem.Percentage
                    };
                    
                    visitChecklistItemRepo.Add(visitChecklistItem);
                }
            }
            
            await _repository.UpdateVisitAsync(visit);
            await _unitOfWork.Complete();
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetMonthlyScheduleAsync(int month, int year)
        {
            return await _repository.GetMonthlyScheduleAsync(month, year);
        }

        public async Task TransferElevatorToMaintenanceAsync(Guid elevatorId, int freeMonths = 6)
        {
            // 1. Fetch Elevator and Project/Customer Info
            var elevator = await _unitOfWork.Repository<LiftOps_BackEnd.Domain.Entities.Installation.Elevator>().GetByIdAsync(elevatorId);
            if (elevator == null) 
            {
                throw new Exception($"Elevator with ID {elevatorId} not found");
            }

            var project = await _unitOfWork.Repository<LiftOps_BackEnd.Domain.Entities.Installation.InstallationProject>().GetByIdAsync(elevator.ProjectId);
            if (project == null) 
            {
                throw new Exception($"Installation project for elevator {elevatorId} not found");
            }

            // 2. Create Maintenance Contract (or find one with matching project number)
            var customerId = project.CustomerId;
            var existingContracts = await GetCustomerContractsAsync(customerId);
            
            // Only reuse a contract if it has the SAME project number as the installation project
            // This ensures each installation project gets its own maintenance contract with the same project number
            MaintenanceContract? contract = null;
            if (!string.IsNullOrWhiteSpace(project.ProjectNumber))
            {
                // Look for existing maintenance contract with the same project number
                contract = existingContracts.FirstOrDefault(c => c.ProjectNumber == project.ProjectNumber);
            }

            if (contract == null)
            {
                // IMPORTANT: When creating a maintenance contract, convert the customer status to Approved (Active)
                // This ensures that customers with maintenance contracts are always in Approved status,
                // regardless of their previous status (e.g., Rejected, PendingInspectionQuotation)
                // Rule: If client was rejected or pending, and we add a new maintenance project for them, they must be Approved
                var customerRepo = _unitOfWork.Repository<Customer>();
                var customer = await customerRepo.GetByIdAsync(customerId);
                if (customer == null)
                {
                    throw new Exception($"Customer with ID {customerId} not found.");
                }
                
                // Set customer status to Approved (Active) when maintenance contract is created
                customer.Status = CustomerStatus.Approved;
                customerRepo.Update(customer);
                
                // Use the same project number as the installation project
                string projectNumber = string.Empty;
                if (!string.IsNullOrWhiteSpace(project.ProjectNumber))
                {
                    // Use the same project number as installation project
                    projectNumber = project.ProjectNumber;
                }
                else
                {
                    // If installation project has no number, generate one
                    projectNumber = await GenerateProjectNumberAsync();
                }

                contract = new MaintenanceContract
                {
                    CustomerId = customerId,
                    ProjectNumber = projectNumber, // Same as installation project number
                    IsFromInstallation = true, // Mark as coming from installation
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1), // Default 1 year
                    PricePerMonth = 0, // Should be set based on some policy or contract details
                    FreeMonths = freeMonths, // Use provided free months (default 6)
                    Status = MaintenanceContractStatus.Active
                };
                _repository.Add(contract);
                await _unitOfWork.Complete();
            }

            // 3. Create Maintenance Elevator
            var mElevator = new MaintenanceElevator
            {
                ContractId = contract.Id,
                Type = elevator.ElevatorType.ToString(),
                NumberOfStops = elevator.NumberOfStops,
                NumberOfFloors = elevator.NumberOfFloors,
                InstallationElevatorId = elevatorId, // Link to original installation elevator
                Status = MaintenanceElevatorStatus.Active
            };

            await AddElevatorToContractAsync(contract.Id, mElevator);
        }

        public async Task<IReadOnlyList<MaintenanceContract>> GetAllContractsAsync()
        {
            return await _repository.GetAllContractsWithDetailsAsync();
        }

        public async Task<MaintenanceContract?> GetContractByIdAsync(Guid contractId)
        {
            return await _repository.GetContractByIdWithDetailsAsync(contractId);
        }

        public async Task<bool> ProjectNumberExistsAsync(string projectNumber)
        {
            if (string.IsNullOrWhiteSpace(projectNumber))
                return false;

            // Check in Maintenance Contracts (same project number can exist in both Installation and Maintenance)
            var maintenanceContracts = await _repository.ListAllAsync();
            if (maintenanceContracts.Any(p => !string.IsNullOrEmpty(p.ProjectNumber) && p.ProjectNumber == projectNumber))
                return true;

            // Also check in Installation Projects (for direct maintenance project creation)
            var installationProjectRepo = _unitOfWork.Repository<InstallationProject>();
            var installationProjects = await installationProjectRepo.ListAllAsync();
            if (installationProjects.Any(p => !string.IsNullOrEmpty(p.ProjectNumber) && p.ProjectNumber == projectNumber))
                return true;

            return false;
        }

        public async Task<MaintenanceContract> CreateMaintenanceProjectAsync(MaintenanceContract contract, List<MaintenanceElevator> elevators)
        {
            // Validate project number is unique
            if (!string.IsNullOrWhiteSpace(contract.ProjectNumber))
            {
                if (await ProjectNumberExistsAsync(contract.ProjectNumber))
                {
                    throw new Exception($"Project number '{contract.ProjectNumber}' already exists. Please use a different project number.");
                }
            }
            else
            {
                // Generate unique project number
                contract.ProjectNumber = await GenerateProjectNumberAsync();
            }

            // Get customer (either from contract.Customer or by CustomerId)
            Customer? customer = null;
            if (contract.Customer != null)
            {
                customer = contract.Customer;
            }
            else if (contract.CustomerId != Guid.Empty)
            {
                var customerRepo = _unitOfWork.Repository<Customer>();
                customer = await customerRepo.GetByIdAsync(contract.CustomerId);
            }

            // Apply fallback logic: If project address/city is not provided, use customer's address/city
            if (customer != null)
            {
                if (string.IsNullOrWhiteSpace(contract.ProjectAddress))
                {
                    contract.ProjectAddress = customer.Address;
                }
                if (string.IsNullOrWhiteSpace(contract.City) || contract.City == "القاهرة الجديدة")
                {
                    contract.City = customer.City;
                }
            }

            // IMPORTANT: Update customer status to Approved when creating a maintenance project
            // Rule: When adding a new client in maintenance, they must be Approved
            // Rule: If client was rejected or pending, and we add a new maintenance project for them, they must be Approved
            Customer? customerToUpdate = null;
            if (contract.Customer != null)
            {
                // Customer is already attached to contract and will be tracked by EF Core when we add the contract
                // Just set the status - don't call Update() as EF Core will save it automatically when saving the contract
                customerToUpdate = contract.Customer;
                customerToUpdate.Status = CustomerStatus.Approved;
            }
            else if (contract.CustomerId != Guid.Empty)
            {
                // Customer is referenced by ID only - fetch and update separately
                var customerRepo = _unitOfWork.Repository<Customer>();
                customerToUpdate = await customerRepo.GetByIdAsync(contract.CustomerId);
                if (customerToUpdate != null)
                {
                    customerToUpdate.Status = CustomerStatus.Approved;
                    customerRepo.Update(customerToUpdate);
                }
            }

            // Create contract
            _repository.Add(contract);
            await _unitOfWork.Complete();

            // Add elevators
            foreach (var elevator in elevators)
            {
                elevator.ContractId = contract.Id;
                await AddElevatorToContractAsync(contract.Id, elevator);
            }

            return contract;
        }

        private async Task<string> GenerateProjectNumberAsync()
        {
            // Generate format: M-YYYY-XXX (e.g., M-2024-001)
            var year = DateTime.UtcNow.Year;
            var contracts = await GetAllContractsAsync();
            var existingNumbers = contracts
                .Where(c => c.ProjectNumber.StartsWith($"M-{year}-"))
                .Select(c => c.ProjectNumber)
                .ToList();

            int sequence = 1;
            string projectNumber;
            do
            {
                projectNumber = $"M-{year}-{sequence:D3}";
                sequence++;
            } while (existingNumbers.Contains(projectNumber) || await ProjectNumberExistsAsync(projectNumber));

            return projectNumber;
        }

        public async Task MarkVisitAsPaidAsync(Guid visitId)
        {
            var visit = await _repository.GetVisitByIdAsync(visitId);
            if (visit == null) throw new Exception("Visit not found");

            visit.IsPaid = true;
            await _repository.UpdateVisitAsync(visit);
            await _unitOfWork.Complete();
        }

        public async Task<IReadOnlyList<MaintenanceElevator>> GetAllElevatorsAsync()
        {
            return await _repository.GetAllElevatorsWithDetailsAsync();
        }

        public async Task<IReadOnlyList<MaintenanceElevator>> GetElevatorsByContractAsync(Guid contractId)
        {
            return await _repository.GetElevatorsByContractAsync(contractId);
        }

        public async Task EnsureContractHasProjectNumberAsync(MaintenanceContract contract)
        {
            if (contract == null) return;
            
            if (string.IsNullOrWhiteSpace(contract.ProjectNumber))
            {
                contract.ProjectNumber = await GenerateProjectNumberAsync();
                _repository.Update(contract);
                await _unitOfWork.Complete();
            }
        }

        public async Task<int> CancelIncompleteVisitsByDateAsync(DateTime date)
        {
            var visits = await _repository.GetVisitsByDateAsync(date);
            
            // Filter visits that are not Done (Pending, InProgress, Frozen)
            var incompleteVisits = visits.Where(v => v.Status != Domain.Entities.Maintenance.VisitStatus.Done && 
                                                     v.Status != Domain.Entities.Maintenance.VisitStatus.Cancelled).ToList();
            
            foreach (var visit in incompleteVisits)
            {
                visit.Status = Domain.Entities.Maintenance.VisitStatus.Cancelled;
                await _repository.UpdateVisitAsync(visit);
            }
            
            await _unitOfWork.Complete();
            return incompleteVisits.Count;
        }

        public async Task UpdateVisitOrderAsync(DateTime date, List<Guid> visitIds)
        {
            // Normalize date to ensure we're comparing dates correctly
            var targetDate = date.Date;
            var visits = await _repository.GetVisitsByDateAsync(targetDate);
            
            if (visits.Count == 0)
            {
                throw new Exception($"No visits found for date {targetDate:yyyy-MM-dd}");
            }
            
            // Update DisplayOrder for each visit based on its position in the ordered list
            for (int i = 0; i < visitIds.Count; i++)
            {
                var visit = visits.FirstOrDefault(v => v.Id == visitIds[i]);
                if (visit != null)
                {
                    visit.DisplayOrder = i;
                    await _repository.UpdateVisitAsync(visit);
                }
            }
            
            // Also set DisplayOrder for any visits on this date that weren't in the list (set to high number so they appear last)
            var visitsNotInList = visits.Where(v => !visitIds.Contains(v.Id)).ToList();
            int maxOrder = visitIds.Count;
            foreach (var visit in visitsNotInList)
            {
                visit.DisplayOrder = maxOrder++;
                await _repository.UpdateVisitAsync(visit);
            }
            
            await _unitOfWork.Complete();
        }

        public async Task<bool> FreezeElevatorAsync(Guid elevatorId, string? reason = null, DateTime? freezeEndDate = null)
        {
            var elevator = await _repository.GetElevatorByIdAsync(elevatorId);
            if (elevator == null) return false;

            elevator.Status = Domain.Entities.Maintenance.MaintenanceElevatorStatus.Frozen;
            await _repository.UpdateElevatorAsync(elevator);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> StopElevatorAsync(Guid elevatorId, string? reason = null)
        {
            var elevator = await _repository.GetElevatorByIdAsync(elevatorId);
            if (elevator == null) return false;

            elevator.Status = Domain.Entities.Maintenance.MaintenanceElevatorStatus.Stopped;
            await _repository.UpdateElevatorAsync(elevator);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> ActivateElevatorAsync(Guid elevatorId)
        {
            var elevator = await _repository.GetElevatorByIdAsync(elevatorId);
            if (elevator == null) return false;

            elevator.Status = Domain.Entities.Maintenance.MaintenanceElevatorStatus.Active;
            await _repository.UpdateElevatorAsync(elevator);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> FreezeContractAsync(Guid contractId, string? reason = null, DateTime? freezeEndDate = null)
        {
            var contract = await _repository.GetByIdAsync(contractId);
            if (contract == null) return false;

            contract.Status = Domain.Entities.Maintenance.MaintenanceContractStatus.Frozen;
            contract.FrozenReason = reason;
            contract.FreezeEndDate = freezeEndDate;
            _repository.Update(contract);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> StopContractAsync(Guid contractId, string? reason = null)
        {
            var contract = await _repository.GetByIdAsync(contractId);
            if (contract == null) return false;

            contract.Status = Domain.Entities.Maintenance.MaintenanceContractStatus.Cancelled;
            contract.FrozenReason = reason;
            _repository.Update(contract);
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> ActivateContractAsync(Guid contractId)
        {
            var contract = await _repository.GetByIdAsync(contractId);
            if (contract == null) return false;

            contract.Status = Domain.Entities.Maintenance.MaintenanceContractStatus.Active;
            contract.FrozenReason = null;
            contract.FreezeEndDate = null;
            _repository.Update(contract);

            // When activating a maintenance contract, ensure customer status is Approved
            // This handles cases where a client was rejected or pending, and we activate their maintenance contract
            var customerRepo = _unitOfWork.Repository<Customer>();
            var customer = await customerRepo.GetByIdAsync(contract.CustomerId);
            if (customer != null)
            {
                customer.Status = CustomerStatus.Approved;
                customerRepo.Update(customer);
            }

            await _unitOfWork.Complete();
            return true;
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByContractAndMonthAsync(Guid contractId, int month, int year)
        {
            return await _repository.GetVisitsByContractAndMonthAsync(contractId, month, year);
        }

        public async Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByElevatorAndMonthAsync(Guid elevatorId, int? month = null, int? year = null)
        {
            return await _repository.GetVisitsByElevatorAndMonthAsync(elevatorId, month, year);
        }

        public async Task<MaintenanceVisit?> GetVisitByIdAsync(Guid visitId)
        {
            return await _repository.GetVisitByIdAsync(visitId);
        }
    }
}
