using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetPlatformDashboardQuery : IRequest<Result<PlatformDashboardDto>>;

public class GetPlatformDashboardQueryHandler : IRequestHandler<GetPlatformDashboardQuery, Result<PlatformDashboardDto>>
{
    private readonly IApplicationDbContext _db;

    public GetPlatformDashboardQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PlatformDashboardDto>> Handle(GetPlatformDashboardQuery request, CancellationToken cancellationToken)
    {
        var companies = _db.Companies.IgnoreQueryFilters();
        var subscriptions = _db.Subscriptions.IgnoreQueryFilters();

        var totalCompanies = await companies.CountAsync(cancellationToken);

        var activeSubscriptions = await subscriptions
            .CountAsync(s => s.Status == SubscriptionStatus.Active, cancellationToken);

        var trialCompanies = await subscriptions
            .CountAsync(s => s.Status == SubscriptionStatus.Trial, cancellationToken);

        var monthlyRevenue = await subscriptions
            .Where(s => s.Status == SubscriptionStatus.Active)
            .Join(_db.SubscriptionPlans.IgnoreQueryFilters(),
                s => s.PlanId,
                p => p.Id,
                (_, p) => p.MonthlyPrice)
            .SumAsync(cancellationToken);

        var companiesByPlan = await subscriptions
            .Join(_db.SubscriptionPlans.IgnoreQueryFilters(),
                s => s.PlanId,
                p => p.Id,
                (_, p) => p.Name)
            .GroupBy(name => name)
            .Select(g => new PlanDistributionDto(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        var dto = new PlatformDashboardDto(
            totalCompanies,
            activeSubscriptions,
            monthlyRevenue,
            trialCompanies,
            new List<RevenueTrendDto>(),
            companiesByPlan);

        return Result<PlatformDashboardDto>.Success(dto);
    }
}
