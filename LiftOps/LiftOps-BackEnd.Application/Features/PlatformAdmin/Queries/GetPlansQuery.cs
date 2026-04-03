using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetPlansQuery : IRequest<Result<List<PlanDto>>>;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, Result<List<PlanDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetPlansQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _db.SubscriptionPlans
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new PlanDto(
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
                p.AllowApiAccess))
            .ToListAsync(cancellationToken);

        return Result<List<PlanDto>>.Success(plans);
    }
}
