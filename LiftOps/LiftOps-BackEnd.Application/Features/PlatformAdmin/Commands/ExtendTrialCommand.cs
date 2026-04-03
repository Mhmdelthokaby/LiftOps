using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record ExtendTrialCommand(Guid CompanyId, ExtendTrialRequest Request) : IRequest<Result>;

public class ExtendTrialCommandHandler : IRequestHandler<ExtendTrialCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public ExtendTrialCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(ExtendTrialCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.Days <= 0)
        {
            return Result.Failure("Days must be greater than zero.");
        }

        var subscription = await _db.Subscriptions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, cancellationToken);

        if (subscription == null)
        {
            return Result.Failure("Subscription not found for company.");
        }

        if (subscription.Status != SubscriptionStatus.Trial)
        {
            PlatformValidation.Throw("Trial extension is only allowed while subscription is in Trial status.");
        }

        if (subscription.CurrentPeriodEnd < DateTime.UtcNow)
        {
            subscription.CurrentPeriodEnd = DateTime.UtcNow;
        }

        subscription.CurrentPeriodEnd = subscription.CurrentPeriodEnd.AddDays(request.Request.Days);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
