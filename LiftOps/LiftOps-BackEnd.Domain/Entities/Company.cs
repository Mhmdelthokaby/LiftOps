using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public bool IsActive { get; set; } = true;
    public string? BillingContactEmail { get; set; }
    public string? Timezone { get; set; }
}
