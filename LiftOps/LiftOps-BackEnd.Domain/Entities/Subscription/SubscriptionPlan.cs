using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Entities.Subscription;

public class SubscriptionPlan : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public int TrialDays { get; set; } = 14;
    public bool IsActive { get; set; } = true;
    public int MaxUsers { get; set; } = 100;
    public int MaxElevators { get; set; } = 100;
    public int MaxMaintenanceContracts { get; set; } = 100;
    public int MaxInstallationProjects { get; set; } = 100;
    public bool AllowEmergencyModule { get; set; } = true;
    public bool AllowFaultsModule { get; set; } = true;
    public bool AllowFinanceModule { get; set; } = true;
    public bool AllowInventoryModule { get; set; } = true;
    public bool AllowApiAccess { get; set; }
    public string? FeatureFlagsJson { get; set; }
}
