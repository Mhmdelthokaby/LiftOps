using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetPlanByIdQuery(Guid Id) : IRequest<Result<PlanDto>>;

public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPlanByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _db.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (p == null)
        {
            return Result<PlanDto>.Failure("Plan not found.");
        }

        var dto = new PlanDto(
            p.Id,
            p.Name,
            p.Code,
            p.MonthlyPrice,
            p.YearlyPrice,
            p.TrialDays,
            p.IsActive,
            p.MaxUsers,
            p.MaxElevators,
            p.MaxMaintenanceContracts,
            p.MaxInstallationProjects,
            p.AllowEmergencyModule,
            p.AllowFaultsModule,
            p.AllowFinanceModule,
            p.AllowInventoryModule,
            p.AllowApiAccess);

        return Result<PlanDto>.Success(dto);
    }
}
