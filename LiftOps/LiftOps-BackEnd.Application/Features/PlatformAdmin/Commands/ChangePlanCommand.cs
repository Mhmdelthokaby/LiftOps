using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record ChangePlanCommand(Guid CompanyId, ChangePlanRequest Request) : IRequest<Result>;

public class ChangePlanCommandHandler : IRequestHandler<ChangePlanCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public ChangePlanCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(ChangePlanCommand request, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken);
        if (company == null)
        {
            return Result.Failure("Company not found.");
        }

        var newPlan = await _db.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Request.NewPlanId, cancellationToken);
        if (newPlan == null)
        {
            return Result.Failure("Plan not found.");
        }

        var userCount = await _db.Users.IgnoreQueryFilters()
            .CountAsync(u => u.CompanyId == request.CompanyId && !u.IsDisabled, cancellationToken);
        var installationElevators = await _db.Elevators.IgnoreQueryFilters()
            .CountAsync(e => e.CompanyId == request.CompanyId, cancellationToken);
        var maintenanceElevators = await _db.MaintenanceElevators.IgnoreQueryFilters()
            .CountAsync(e => e.CompanyId == request.CompanyId, cancellationToken);
        var elevatorCount = installationElevators + maintenanceElevators;
        var contractCount = await _db.MaintenanceContracts.IgnoreQueryFilters()
            .CountAsync(c => c.CompanyId == request.CompanyId, cancellationToken);
        var projectCount = await _db.InstallationProjects.IgnoreQueryFilters()
            .CountAsync(p => p.CompanyId == request.CompanyId, cancellationToken);

        if (userCount > newPlan.MaxUsers)
        {
            PlatformValidation.Throw($"Cannot downgrade: company has {userCount} users, new plan allows {newPlan.MaxUsers}.");
        }

        if (elevatorCount > newPlan.MaxElevators)
        {
            PlatformValidation.Throw($"Cannot downgrade: company has {elevatorCount} elevators, new plan allows {newPlan.MaxElevators}.");
        }

        if (contractCount > newPlan.MaxMaintenanceContracts)
        {
            PlatformValidation.Throw($"Cannot downgrade: company has {contractCount} maintenance contracts, new plan allows {newPlan.MaxMaintenanceContracts}.");
        }

        if (projectCount > newPlan.MaxInstallationProjects)
        {
            PlatformValidation.Throw($"Cannot downgrade: company has {projectCount} installation projects, new plan allows {newPlan.MaxInstallationProjects}.");
        }

        var subscription = await _db.Subscriptions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId, cancellationToken);
        if (subscription == null)
        {
            return Result.Failure("Subscription not found for company.");
        }

        subscription.PlanId = newPlan.Id;
        var start = DateTime.UtcNow;
        subscription.CurrentPeriodStart = start;
        subscription.CurrentPeriodEnd = subscription.BillingCycle == SubscriptionBillingCycle.Yearly
            ? start.AddYears(1)
            : start.AddMonths(1);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
