using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; } = true;
    public string? BillingContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Timezone { get; set; }
    public CompanyTenantStatus TenantStatus { get; set; } = CompanyTenantStatus.Active;
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
}
