using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Entities.Subscription;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin;

internal static class PlatformAdminMapper
{
    public static string ToCompanyStatusString(Company company) =>
        company.TenantStatus switch
        {
            CompanyTenantStatus.Suspended => "Suspended",
            CompanyTenantStatus.SuspendedByAdmin => "SuspendedByAdmin",
            CompanyTenantStatus.Deleted => "Deleted",
            _ => company.IsActive ? "Active" : "Suspended"
        };

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
