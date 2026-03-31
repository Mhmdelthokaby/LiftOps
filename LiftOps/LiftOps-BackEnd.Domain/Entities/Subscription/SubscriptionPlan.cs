using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Entities.Subscription;

public class SubscriptionPlan : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public string? FeatureFlagsJson { get; set; }
}
