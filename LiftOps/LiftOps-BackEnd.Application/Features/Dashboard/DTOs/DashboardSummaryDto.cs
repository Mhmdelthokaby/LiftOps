using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Dashboard.DTOs;

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }
    public int ActiveInstallations { get; set; }
    public int MaintenanceDue { get; set; }
    public int OpenEmergencies { get; set; }
    public int LowStockItems { get; set; }

    public List<RevenueDataPoint> RevenueData { get; set; } = new();
    public List<ProjectStatusDataPoint> ProjectStatusData { get; set; } = new();
    public List<ActivityItemDto> RecentActivities { get; set; } = new();
}

public class RevenueDataPoint
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
}

public class ProjectStatusDataPoint
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class ActivityItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "completion", "emergency", "maintenance", "inventory"
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } // Frontend will convert to "2 hours ago"
    public string? Badge { get; set; }
}
