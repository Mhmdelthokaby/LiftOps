using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record CreatePlanCommand(CreatePlanRequest Request) : IRequest<Result<PlanDto>>;

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, Result<PlanDto>>
{
    private readonly IApplicationDbContext _db;

    public CreatePlanCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PlanDto>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var code = req.Code.Trim();
        var exists = await _db.SubscriptionPlans.AnyAsync(p => p.Code == code, cancellationToken);
        if (exists)
        {
            return Result<PlanDto>.Failure("A plan with this code already exists.");
        }

        var plan = new SubscriptionPlan
        {
            Name = req.Name.Trim(),
            Code = code,
            MonthlyPrice = req.MonthlyPrice,
            YearlyPrice = req.YearlyPrice,
            TrialDays = req.TrialDays,
            IsActive = true,
            MaxUsers = req.MaxUsers,
            MaxElevators = req.MaxElevators,
            MaxMaintenanceContracts = req.MaxMaintenanceContracts,
            MaxInstallationProjects = req.MaxInstallationProjects,
            AllowEmergencyModule = req.AllowEmergencyModule,
            AllowFaultsModule = req.AllowFaultsModule,
            AllowFinanceModule = req.AllowFinanceModule,
            AllowInventoryModule = req.AllowInventoryModule,
            AllowApiAccess = req.AllowApiAccess
        };

        _db.SubscriptionPlans.Add(plan);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new PlanDto(
            plan.Id,
            plan.Name,
            plan.Code,
            plan.MonthlyPrice,
            plan.YearlyPrice,
            plan.TrialDays,
            plan.IsActive,
            plan.MaxUsers,
            plan.MaxElevators,
            plan.MaxMaintenanceContracts,
            plan.MaxInstallationProjects,
            plan.AllowEmergencyModule,
            plan.AllowFaultsModule,
            plan.AllowFinanceModule,
            plan.AllowInventoryModule,
            plan.AllowApiAccess);

        return Result<PlanDto>.Success(dto);
    }
}
