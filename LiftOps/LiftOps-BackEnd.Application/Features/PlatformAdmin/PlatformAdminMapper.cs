using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Entities.Subscription;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin;

internal static class PlatformAdminMapper
{
    public static string ToCompanyStatusString(Company company) =>
        ToCompanyStatusString(company.TenantStatus, company.IsActive, company.IsDeleted);

    public static string ToCompanyStatusString(CompanyTenantStatus tenantStatus, bool isActive, bool isDeleted)
    {
        if (isDeleted || tenantStatus == CompanyTenantStatus.Deleted)
        {
            return "Deleted";
        }

        return tenantStatus switch
        {
            CompanyTenantStatus.Suspended => "Suspended",
            CompanyTenantStatus.SuspendedByAdmin => "SuspendedByAdmin",
            _ => isActive ? "Active" : "Inactive"
        };
    }

    public static string ToSubscriptionStatusString(SubscriptionStatus status, DateTime currentPeriodEnd)
    {
        if (status == SubscriptionStatus.Active && currentPeriodEnd < DateTime.UtcNow)
        {
            return "Expired";
        }

        return status switch
        {
            SubscriptionStatus.Trial => "Trial",
            SubscriptionStatus.Active => "Active",
            SubscriptionStatus.PastDue => "PastDue",
            SubscriptionStatus.Cancelled => "Cancelled",
            _ => "Cancelled"
        };
    }

    public static string ToBillingCycleString(SubscriptionBillingCycle cycle) =>
        cycle == SubscriptionBillingCycle.Yearly ? "Yearly" : "Monthly";

    public static void SplitFullName(string fullName, out string first, out string last)
    {
        fullName ??= string.Empty;
        var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        first = parts.Length > 0 ? parts[0] : string.Empty;
        last = parts.Length > 1 ? parts[1] : string.Empty;
    }
}
