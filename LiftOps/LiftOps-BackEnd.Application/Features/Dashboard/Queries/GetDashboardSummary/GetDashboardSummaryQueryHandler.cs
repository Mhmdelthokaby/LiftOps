using LiftOps_BackEnd.Application.Features.Dashboard.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftOps_BackEnd.Domain.Entities.Faults;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using LiftOps_BackEnd.Domain.Interfaces.Maintenance;

namespace LiftOps_BackEnd.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenanceRepository _maintenanceRepository;

    public GetDashboardSummaryQueryHandler(IUnitOfWork unitOfWork, IMaintenanceRepository maintenanceRepository)
    {
        _unitOfWork = unitOfWork;
        _maintenanceRepository = maintenanceRepository;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch Data
        var projects = await _unitOfWork.Repository<InstallationProject>().ListAllAsync();
        var maintenanceVisits = await _unitOfWork.Repository<MaintenanceVisit>().ListAllAsync();
        var maintenanceContracts = await _unitOfWork.Repository<MaintenanceContract>().ListAllAsync();
        var maintenanceElevators = await _unitOfWork.Repository<MaintenanceElevator>().ListAllAsync();
        var faults = await _unitOfWork.Repository<FaultTicket>().ListAllAsync();
        var inventory = await _unitOfWork.Repository<InventoryItem>().ListAllAsync();

        // 2. KPI Calculations
        var totalProjects = projects.Count;
        
        var activeInstallations = projects.Count(p => p.InstallationStartDate.HasValue && (!p.ExpectedFinishDate.HasValue || p.ExpectedFinishDate.Value > DateTime.Now));

        // Calculate maintenance due: contracts that have elevators without completed visits for current month
        // Use local time to match how VisitDate is likely stored
        var now = DateTime.Now;
        var currentMonth = now.Month;
        var currentYear = now.Year;
        
        // Get all Active maintenance contracts with elevators included
        // A maintenance contract can be Active even if the installation project is Rejected
        var allContractsWithElevators = await _maintenanceRepository.GetAllContractsWithDetailsAsync();
        var activeContractsWithElevators = allContractsWithElevators
            .Where(c => c.Status == MaintenanceContractStatus.Active)
            .ToList();
        
        // Get all completed visits for current month
        var allVisits = await _unitOfWork.Repository<MaintenanceVisit>().ListAllAsync();
        var completedVisitsThisMonth = allVisits
            .Where(v => v.Status == VisitStatus.Done
                     && v.VisitDate.Date.Year == currentYear
                     && v.VisitDate.Date.Month == currentMonth)
            .Select(v => v.MaintenanceElevatorId)
            .ToList();
        
        // Count contracts that have at least one ACTIVE elevator without completed visit this month
        var maintenanceDue = 0;
        foreach (var contract in activeContractsWithElevators)
        {
            // Filter to only ACTIVE elevators (exclude Frozen and Stopped)
            var activeElevators = contract.Elevators?
                .Where(e => e.Status == MaintenanceElevatorStatus.Active)
                .ToList() ?? new List<MaintenanceElevator>();
            
            // Skip contracts with no active elevators
            if (!activeElevators.Any())
                continue;
            
            // Check if contract has at least one ACTIVE elevator without completed visit this month
            var hasUnmaintainedActiveElevator = activeElevators
                .Any(elevator => !completedVisitsThisMonth.Contains(elevator.Id));
            
            // If contract has at least one active elevator without completed visit, it's due
            if (hasUnmaintainedActiveElevator)
            {
                maintenanceDue++;
            }
        }

        var openEmergencies = faults.Count(f => f.Status == TicketStatus.Pending || f.Status == TicketStatus.InProgress);

        var lowStockItems = inventory.Count(i => i.StockQuantity < 10);

        // 3. Revenue Data (Mocked for now as per plan, but structured dynamically)
        // In real app, query Finance module transactions.
        var revenueData = new List<RevenueDataPoint>
        {
            new() { Month = "Jan", Revenue = 145000, Expenses = 98000 },
            new() { Month = "Feb", Revenue = 168000, Expenses = 102000 },
            new() { Month = "Mar", Revenue = 192000, Expenses = 115000 },
            new() { Month = "Apr", Revenue = 178000, Expenses = 108000 },
            new() { Month = "May", Revenue = 205000, Expenses = 122000 },
            new() { Month = "Jun", Revenue = 198000, Expenses = 118000 },
            new() { Month = "Jul", Revenue = 215000, Expenses = 125000 },
            new() { Month = "Aug", Revenue = 228000, Expenses = 132000 },
            new() { Month = "Sep", Revenue = 242000, Expenses = 138000 },
            new() { Month = "Oct", Revenue = 235000, Expenses = 135000 },
            new() { Month = "Nov", Revenue = 258000, Expenses = 145000 },
            new() { Month = "Dec", Revenue = 275000, Expenses = 152000 },
        };

        // 4. Project Status Data
        // Simplified Logic: 
        // Completed: IsCompleted or EndDate passed
        // In Progress: active
        // Other: remaining projects
        
        var completedProjects = projects.Count(p => p.ExpectedFinishDate.HasValue && p.ExpectedFinishDate.Value < DateTime.Now);
        var inProgresProjects = activeInstallations;
        var onHoldProjects = projects.Count - (completedProjects + inProgresProjects); 
        if (onHoldProjects < 0) onHoldProjects = 0; // simple fix for overlap

        var projectStatusData = new List<ProjectStatusDataPoint>
        {
            new() { Name = "Completed", Value = completedProjects, Color = "hsl(var(--chart-3))" },
            new() { Name = "In Progress", Value = inProgresProjects, Color = "hsl(var(--chart-1))" },
            new() { Name = "Other", Value = onHoldProjects, Color = "hsl(var(--chart-5))" },
        };

        // 5. Activity Feed
        var activities = new List<ActivityItemDto>();

        // New Projects
        foreach(var p in projects.OrderByDescending(x => x.CreatedAt).Take(5))
        {
             activities.Add(new ActivityItemDto
             {
                 Id = p.Id.ToString(),
                 Type = "completion", // Using generic type for project updates
                 Title = "New Project Started",
                 Description = $"{p.Notes} (Contract Date: {p.ContractDate:d})",
                 Timestamp = p.CreatedAt,
                 Badge = "New"
             });
        }

        // Faults
        foreach(var f in faults.OrderByDescending(x => x.FaultDate).Take(5))
        {
            activities.Add(new ActivityItemDto
            {
                Id = f.Id.ToString(),
                Type = "emergency",
                Title = $"Fault Ticket {f.TicketNumber}",
                Description = $"{f.FaultDescription}",
                Timestamp = f.FaultDate,
                Badge = f.Severity == FaultSeverity.High ? "High Priority" : "Issue"
            });
        }

        // Maintenance
        foreach(var m in maintenanceVisits.OrderByDescending(x => x.VisitDate).Take(5))
        {
            activities.Add(new ActivityItemDto
            {
                Id = m.Id.ToString(),
                Type = "maintenance",
                Title = "Maintenance Visit",
                Description = $"Status: {m.Status}",
                Timestamp = m.VisitDate,
                Badge = m.Status.ToString()
            });
        }

        // Sort and take top 10
        var recentActivities = activities.OrderByDescending(a => a.Timestamp).Take(10).ToList();

        return new DashboardSummaryDto
        {
            TotalProjects = totalProjects,
            ActiveInstallations = activeInstallations,
            MaintenanceDue = maintenanceDue,
            OpenEmergencies = openEmergencies,
            LowStockItems = lowStockItems,
            RevenueData = revenueData,
            ProjectStatusData = projectStatusData,
            RecentActivities = recentActivities
        };
    }
}
