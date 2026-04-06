using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities.Subscription;

namespace LiftOps_BackEnd.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Soft delete: hidden from default lists; tenant data retained.</summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>FK to <see cref="SubscriptionPlan"/> (catalog plan for this tenant).</summary>
    public Guid? PlanId { get; set; }
    public SubscriptionPlan? Plan { get; set; }

    public string? BillingContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Timezone { get; set; }
    public CompanyTenantStatus TenantStatus { get; set; } = CompanyTenantStatus.Active;
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
}
