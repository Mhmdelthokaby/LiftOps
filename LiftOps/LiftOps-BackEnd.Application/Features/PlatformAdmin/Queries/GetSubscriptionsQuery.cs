using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetSubscriptionsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null,
    Guid? PlanId = null) : IRequest<Result<PaginatedResultDto<SubscriptionListDto>>>;

public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, Result<PaginatedResultDto<SubscriptionListDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetSubscriptionsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResultDto<SubscriptionListDto>>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var q = from s in _db.Subscriptions.IgnoreQueryFilters().AsNoTracking()
                join c in _db.Companies.IgnoreQueryFilters().AsNoTracking() on s.CompanyId equals c.Id
                join p in _db.SubscriptionPlans.IgnoreQueryFilters().AsNoTracking() on s.PlanId equals p.Id
                select new { Subscription = s, CompanyName = c.Name, PlanName = p.Name, PlanMonthly = p.MonthlyPrice, PlanYearly = p.YearlyPrice };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.CompanyName.Contains(s) || x.PlanName.Contains(s));
        }

        if (request.PlanId.HasValue)
        {
            var pid = request.PlanId.Value;
            q = q.Where(x => x.Subscription.PlanId == pid);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var st = request.Status.Trim();
            q = st switch
            {
                "Trial" => q.Where(x => x.Subscription.Status == SubscriptionStatus.Trial),
                "Active" => q.Where(x => x.Subscription.Status == SubscriptionStatus.Active),
                "PastDue" => q.Where(x => x.Subscription.Status == SubscriptionStatus.PastDue),
                "Cancelled" => q.Where(x => x.Subscription.Status == SubscriptionStatus.Cancelled),
                "Expired" => q.Where(x => x.Subscription.Status == SubscriptionStatus.Active && x.Subscription.CurrentPeriodEnd < DateTime.UtcNow),
                _ => q
            };
        }

        var total = await q.CountAsync(cancellationToken);

        var rows = await q
            .OrderByDescending(x => x.Subscription.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var items = rows.Select(x =>
        {
            var s = x.Subscription;
            var amount = s.BillingCycle == SubscriptionBillingCycle.Yearly ? x.PlanYearly : x.PlanMonthly;
            var daysRemaining = (int)Math.Max(0, (s.CurrentPeriodEnd.Date - now.Date).TotalDays);
            return new SubscriptionListDto(
                s.Id,
                s.CompanyId,
                x.CompanyName,
                x.PlanName,
                PlatformAdminMapper.ToSubscriptionStatusString(s.Status, s.CurrentPeriodEnd),
                PlatformAdminMapper.ToBillingCycleString(s.BillingCycle),
                s.CurrentPeriodStart,
                s.CurrentPeriodEnd,
                daysRemaining,
                amount);
        }).ToList();

        return Result<PaginatedResultDto<SubscriptionListDto>>.Success(
            new PaginatedResultDto<SubscriptionListDto>(items, total, page, pageSize));
    }
}
