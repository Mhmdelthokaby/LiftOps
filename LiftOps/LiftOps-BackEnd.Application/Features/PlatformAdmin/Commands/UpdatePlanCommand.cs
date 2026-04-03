using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record UpdatePlanCommand(Guid Id, UpdatePlanRequest Request) : IRequest<Result<PlanDto>>;

public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, Result<PlanDto>>
{
    private readonly IApplicationDbContext _db;

    public UpdatePlanCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PlanDto>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (plan == null)
        {
            return Result<PlanDto>.Failure("Plan not found.");
        }

        var req = request.Request;
        var code = req.Code.Trim();
        var codeTaken = await _db.SubscriptionPlans
            .AnyAsync(p => p.Code == code && p.Id != plan.Id, cancellationToken);
        if (codeTaken)
        {
            return Result<PlanDto>.Failure("A plan with this code already exists.");
        }

        plan.Name = req.Name.Trim();
        plan.Code = code;
        plan.MonthlyPrice = req.MonthlyPrice;
        plan.YearlyPrice = req.YearlyPrice;
        plan.TrialDays = req.TrialDays;
        plan.IsActive = req.IsActive;
        plan.MaxUsers = req.MaxUsers;
        plan.MaxElevators = req.MaxElevators;
        plan.MaxMaintenanceContracts = req.MaxMaintenanceContracts;
        plan.MaxInstallationProjects = req.MaxInstallationProjects;
        plan.AllowEmergencyModule = req.AllowEmergencyModule;
        plan.AllowFaultsModule = req.AllowFaultsModule;
        plan.AllowFinanceModule = req.AllowFinanceModule;
        plan.AllowInventoryModule = req.AllowInventoryModule;
        plan.AllowApiAccess = req.AllowApiAccess;

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
