using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record DeletePlanCommand(Guid Id) : IRequest<Result>;

public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeletePlanCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (plan == null)
        {
            return Result.Failure("Plan not found.");
        }

        var inUse = await _db.Subscriptions.IgnoreQueryFilters()
            .AnyAsync(s => s.PlanId == plan.Id, cancellationToken);
        if (inUse)
        {
            PlatformValidation.Throw("Cannot delete a plan that is assigned to one or more companies.");
        }

        _db.SubscriptionPlans.Remove(plan);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
