using LiftOps_BackEnd.Domain.Entities.Subscription;

namespace LiftOps_BackEnd.Application.Interfaces;

public interface ISubscriptionLifecycleService
{
    Task EnsureTrialSubscriptionAsync(Guid companyId, CancellationToken cancellationToken);
    Task<bool> MarkPastDueAsync(Guid companyId, string? externalCustomerId, CancellationToken cancellationToken);
    Task<bool> ExtendTrialAsync(Guid companyId, int days, CancellationToken cancellationToken);
    Task<bool> SetStatusAsync(Guid companyId, SubscriptionStatus status, CancellationToken cancellationToken);
}
