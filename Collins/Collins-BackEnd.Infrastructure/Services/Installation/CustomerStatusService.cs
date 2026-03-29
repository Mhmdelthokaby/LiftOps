using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Entities.Maintenance;
using Collins_BackEnd.Domain.Interfaces;
using Collins_BackEnd.Domain.Interfaces.Maintenance;
using Collins_BackEnd.Infrastructure.Persistence;
using Collins_BackEnd.Application.Interfaces.Installation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Services.Installation
{
    /// <summary>
    /// Service to calculate customer status dynamically based on their projects and maintenance contracts
    /// </summary>
    public class CustomerStatusService : ICustomerStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly IMaintenanceRepository _maintenanceRepository;

        public CustomerStatusService(IUnitOfWork unitOfWork, ApplicationDbContext context, IMaintenanceRepository maintenanceRepository)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _maintenanceRepository = maintenanceRepository;
        }

        /// <summary>
        /// Gets the calculated customer status for a customer based on their projects and maintenance contracts
        /// This is used to check status without updating the database
        /// </summary>
        public async Task<CustomerStatus> GetCalculatedCustomerStatusAsync(Guid customerId)
        {
            // Get all projects for this customer with elevators and stages loaded
            var customerProjects = await _context.InstallationProjects
                .Include(p => p.Elevators)
                    .ThenInclude(e => e.Stages)
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();

            // Get all maintenance contracts for this customer
            var maintenanceContracts = await _maintenanceRepository.GetContractsByCustomerAsync(customerId);

            return CalculateCustomerStatus(customerProjects, maintenanceContracts);
        }

        /// <summary>
        /// Calculates customer status based on all their projects and maintenance contracts
        /// Rules:
        /// - If client has at least one active maintenance contract → Client status = "Approved"
        /// - If client has at least one project that is "active", "inprogress", or "completed" → Client status = "Approved"
        /// - If client has at least one project with status "UnderInspectionAndQuotation" (pending) → Client status = "PendingInspectionQuotation"
        /// - If client has ONLY rejected projects (no pending or active projects) → Client status = "Rejected"
        /// - If rejected client has a new project that gets approved → Client status changes from "Rejected" to "Approved"
        /// </summary>
        public CustomerStatus CalculateCustomerStatus(List<InstallationProject> projects, IReadOnlyList<MaintenanceContract> maintenanceContracts = null)
        {
            // First check: If client has at least one active maintenance contract, they are Approved
            if (maintenanceContracts != null && maintenanceContracts.Any(c => c.Status == MaintenanceContractStatus.Active))
            {
                return CustomerStatus.Approved;
            }

            if (projects == null || projects.Count == 0)
            {
                return CustomerStatus.PendingInspectionQuotation;
            }

            // Check if any project is active, in progress, or completed
            // These statuses indicate the client is approved/active
            bool hasActiveProject = projects.Any(p => IsProjectActiveOrInProgressOrCompleted(p));
            
            if (hasActiveProject)
            {
                return CustomerStatus.Approved;
            }

            // Check if any project is pending inspection/quotation
            // If there's a pending project, client should be PendingInspectionQuotation (not Rejected)
            bool hasPendingProject = projects.Any(p => p.ProjectStatus == ProjectStatus.UnderInspectionAndQuotation);
            
            if (hasPendingProject)
            {
                return CustomerStatus.PendingInspectionQuotation;
            }

            // Check if all projects are rejected
            // Only mark as Rejected if ALL projects are rejected AND there are no pending/active projects
            bool allRejected = projects.All(p => p.ProjectStatus == ProjectStatus.Rejected);
            
            if (allRejected)
            {
                return CustomerStatus.Rejected;
            }

            // Default to pending if we can't determine (shouldn't happen, but safety fallback)
            return CustomerStatus.PendingInspectionQuotation;
        }

        /// <summary>
        /// Checks if a project is considered "active" (approved/active/inprogress/completed)
        /// This means the project is approved and either has no stages yet, has stages in progress, or is completed
        /// </summary>
        private bool IsProjectActiveOrInProgressOrCompleted(InstallationProject project)
        {
            // Project must be approved or active (not rejected or pending)
            if (project.ProjectStatus != ProjectStatus.Approved && project.ProjectStatus != ProjectStatus.Active)
            {
                return false;
            }

            // If project has no elevators, it's considered active
            if (project.Elevators == null || project.Elevators.Count == 0)
            {
                return true;
            }

            // Check if any elevator has stages that are in progress or completed
            foreach (var elevator in project.Elevators)
            {
                if (elevator.Stages == null || elevator.Stages.Count == 0)
                {
                    // No stages yet, but project is approved/active, so it's active
                    return true;
                }

                // Check if any stage is in progress or all stages are completed
                bool hasInProgress = elevator.Stages.Any(s => s.Status == StageStatus.InProgress);
                bool allCompleted = elevator.Stages.All(s => s.Status == StageStatus.Success);
                
                if (hasInProgress || allCompleted)
                {
                    return true;
                }
            }

            // Project is approved/active but stages haven't started yet - still considered active
            return true;
        }

        /// <summary>
        /// Recalculates and updates customer status based on all their projects and maintenance contracts
        /// </summary>
        public async Task UpdateCustomerStatusAsync(Guid customerId)
        {
            var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(customerId);
            if (customer == null) return;

            // Get all projects for this customer with elevators and stages loaded
            var customerProjects = await _context.InstallationProjects
                .Include(p => p.Elevators)
                    .ThenInclude(e => e.Stages)
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();

            // Get all maintenance contracts for this customer
            var maintenanceContracts = await _maintenanceRepository.GetContractsByCustomerAsync(customerId);

            // Calculate new status
            var newStatus = CalculateCustomerStatus(customerProjects, maintenanceContracts);

            // Update customer status if it changed
            if (customer.Status != newStatus)
            {
                customer.Status = newStatus;
                _unitOfWork.Repository<Customer>().Update(customer);
                await _unitOfWork.Complete();
            }
        }
    }
}

