using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LiftOps_BackEnd.Infrastructure.Services;

public class SubscriptionLifecycleService : ISubscriptionLifecycleService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public SubscriptionLifecycleService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task EnsureTrialSubscriptionAsync(Guid companyId, CancellationToken cancellationToken)
    {
        using var _ = _context.UseSystemTenantBypass(companyId);

        var exists = await _context.Subscriptions.AnyAsync(s => s.CompanyId == companyId, cancellationToken);
        if (exists)
        {
            return;
        }

        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Code == "basic", cancellationToken);
        if (plan == null)
        {
            plan = new SubscriptionPlan
            {
                Code = "basic",
                Name = "Basic",
                MonthlyPrice = 0m,
                FeatureFlagsJson = "{}"
            };
            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var trialDays = _configuration.GetValue("Subscription:TrialDays", 14);
        var now = DateTime.UtcNow;
        _context.Subscriptions.Add(new Subscription
        {
            CompanyId = companyId,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Trial,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = now.AddDays(trialDays),
            BillingCycle = SubscriptionBillingCycle.Monthly
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> MarkPastDueAsync(Guid companyId, string? externalCustomerId, CancellationToken cancellationToken)
    {
        using var _ = _context.UseSystemTenantBypass(companyId);
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.CompanyId == companyId, cancellationToken);
        if (subscription == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(externalCustomerId))
        {
            subscription.ExternalCustomerId = externalCustomerId;
        }

        subscription.Status = SubscriptionStatus.PastDue;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExtendTrialAsync(Guid companyId, int days, CancellationToken cancellationToken)
    {
        using var _ = _context.UseSystemTenantBypass(companyId);
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.CompanyId == companyId, cancellationToken);
        if (subscription == null)
        {
            return false;
        }

        if (subscription.CurrentPeriodEnd < DateTime.UtcNow)
        {
            subscription.CurrentPeriodEnd = DateTime.UtcNow;
        }

        subscription.CurrentPeriodEnd = subscription.CurrentPeriodEnd.AddDays(days);
        if (subscription.Status == SubscriptionStatus.PastDue || subscription.Status == SubscriptionStatus.Cancelled)
        {
            subscription.Status = SubscriptionStatus.Trial;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetStatusAsync(Guid companyId, SubscriptionStatus status, CancellationToken cancellationToken)
    {
        using var _ = _context.UseSystemTenantBypass(companyId);
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.CompanyId == companyId, cancellationToken);
        if (subscription == null)
        {
            return false;
        }

        subscription.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
