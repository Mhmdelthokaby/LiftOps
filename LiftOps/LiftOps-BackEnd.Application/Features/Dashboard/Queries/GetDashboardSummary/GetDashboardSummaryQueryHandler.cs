using LiftOps_BackEnd.Application.Features.Dashboard.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using LiftOps_BackEnd.Domain.Entities.Emergency;
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
        var emergencyTickets = await _unitOfWork.Repository<EmergencyTicket>().ListAllAsync();
        var faults = await _unitOfWork.Repository<FaultTicket>().ListAllAsync();
        var maintenanceSparePartUsages = await _unitOfWork.Repository<MaintenanceSparePartUsage>().ListAllAsync();
        var faultSparePartUsages = await _unitOfWork.Repository<FaultSparePartUsage>().ListAllAsync();
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

        // KPI: open emergencies must be based on EmergencyTickets (not FaultTickets)
        var openEmergencies = emergencyTickets.Count(t =>
            t.Status == EmergencyStatus.Open || t.Status == EmergencyStatus.InProgress);

        var lowStockItems = inventory.Count(i => i.StockQuantity < 10);

        // 3. Revenue Data
        // Revenue is approximated from MaintenanceContract.PricePerMonth for active contracts.
        // Expenses are approximated from spare-part usage costs (MaintenanceSparePartUsage + FaultSparePartUsage).
        // This removes hardcoded/mock month-by-month values while still using available domain data.
        var revenueByMonth = new decimal[13]; // 1..12
        var expensesByMonth = new decimal[13]; // 1..12

        var year = now.Year;

        // Map visit & fault dates so we can attribute usage to a month.
        var maintenanceVisitDateById = maintenanceVisits.ToDictionary(v => v.Id, v => v.VisitDate);
        var faultDateById = faults.ToDictionary(f => f.Id, f => f.FaultDate);

        // Revenue: sum PricePerMonth of active contracts overlapping each month.
        for (var month = 1; month <= 12; month++)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            var revenueForMonth = maintenanceContracts
                .Where(c =>
                    c.Status == MaintenanceContractStatus.Active &&
                    c.StartDate <= monthEnd &&
                    c.EndDate >= monthStart)
                .Sum(c => c.PricePerMonth);

            revenueByMonth[month] = revenueForMonth;
        }

        // Expenses: maintenance spare-part usages
        foreach (var usage in maintenanceSparePartUsages)
        {
            if (!maintenanceVisitDateById.TryGetValue(usage.MaintenanceVisitId, out var visitDate))
                continue;
            if (visitDate.Year != year)
                continue;

            var month = visitDate.Month;
            expensesByMonth[month] += usage.Quantity * usage.PriceAtTimeOfUsage;
        }

        // Expenses: fault spare-part usages
        foreach (var usage in faultSparePartUsages)
        {
            if (!faultDateById.TryGetValue(usage.FaultTicketId, out var faultDate))
                continue;
            if (faultDate.Year != year)
                continue;

            var month = faultDate.Month;
            expensesByMonth[month] += usage.Quantity * usage.PriceAtTimeOfUsage;
        }

        var revenueData = Enumerable.Range(1, 12)
            .Select(m => new RevenueDataPoint
            {
                Month = new DateTime(year, m, 1).ToString("MMM", CultureInfo.InvariantCulture),
                Revenue = revenueByMonth[m],
                Expenses = expensesByMonth[m]
            })
            .ToList();

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
