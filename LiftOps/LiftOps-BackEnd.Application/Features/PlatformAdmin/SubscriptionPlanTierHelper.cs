using LiftOps_BackEnd.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin;

internal static class SubscriptionPlanTierHelper
{
    /// <summary>Maps UI tier labels to likely <see cref="SubscriptionPlan.Code"/> values in the database.</summary>
    public static async Task<Guid?> ResolveActivePlanIdAsync(
        IApplicationDbContext db,
        string tierLabel,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tierLabel))
        {
            return null;
        }

        var t = tierLabel.Trim();
        var codeCandidates = t.Equals("Free", StringComparison.OrdinalIgnoreCase)
            ? new[] { "free", "basic", "starter" }
            : t.Equals("Pro", StringComparison.OrdinalIgnoreCase)
                ? new[] { "pro", "professional" }
                : t.Equals("Enterprise", StringComparison.OrdinalIgnoreCase)
                    ? new[] { "enterprise", "ent" }
                    : new[] { t.ToLowerInvariant() };

        foreach (var code in codeCandidates)
        {
            var plan = await db.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IsActive && p.Code.ToLower() == code, cancellationToken);
            if (plan != null)
            {
                return plan.Id;
            }
        }

        return await db.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.MonthlyPrice)
            .FirstOrDefaultAsync(p => p.Name.Contains(t), cancellationToken)
            is { } byName
            ? byName.Id
            : null;
    }
}
