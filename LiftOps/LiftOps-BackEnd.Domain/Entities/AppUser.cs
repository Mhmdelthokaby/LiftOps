using Microsoft.AspNetCore.Identity;

namespace LiftOps_BackEnd.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    /// <summary>Null for platform administrators (not bound to a tenant).</summary>
    public Guid? CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
