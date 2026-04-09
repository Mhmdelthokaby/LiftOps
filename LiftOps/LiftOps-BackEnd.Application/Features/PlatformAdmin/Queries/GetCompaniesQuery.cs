using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetCompaniesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Status = null,
    Guid? PlanId = null) : IRequest<Result<PaginatedResultDto<CompanyListDto>>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, Result<PaginatedResultDto<CompanyListDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCompaniesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PaginatedResultDto<CompanyListDto>>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var q = _db.Companies.IgnoreQueryFilters().AsNoTracking();

        if (string.IsNullOrWhiteSpace(request.Status) ||
            !string.Equals(request.Status.Trim(), "Deleted", StringComparison.OrdinalIgnoreCase))
        {
            q = q.Where(c => !c.IsDeleted);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(c => c.Name.Contains(s) || (c.BillingContactEmail != null && c.BillingContactEmail.Contains(s)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var st = request.Status.Trim();
            q = st switch
            {
                "Active" => q.Where(c => !c.IsDeleted && c.TenantStatus == CompanyTenantStatus.Active && c.IsActive),
                "Suspended" => q.Where(c => !c.IsDeleted && (c.TenantStatus == CompanyTenantStatus.Suspended || (!c.IsActive && c.TenantStatus == CompanyTenantStatus.Active))),
                "SuspendedByAdmin" => q.Where(c => !c.IsDeleted && c.TenantStatus == CompanyTenantStatus.SuspendedByAdmin),
                "Inactive" => q.Where(c => !c.IsDeleted && !c.IsActive && c.TenantStatus == CompanyTenantStatus.Active),
                "Deleted" => q.Where(c => c.IsDeleted || c.TenantStatus == CompanyTenantStatus.Deleted),
                _ => q
            };
        }

        if (request.PlanId is { } planFilter && planFilter != Guid.Empty)
        {
            var companyIdsForPlan = await _db.Subscriptions.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(s => s.PlanId == planFilter)
                .Select(s => s.CompanyId)
                .Distinct()
                .ToListAsync(cancellationToken);
            q = q.Where(c => companyIdsForPlan.Contains(c.Id));
        }

        var total = await q.CountAsync(cancellationToken);

        var companyRows = await q
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.BillingContactEmail,
                c.ContactPhone,
                c.CreatedAt,
                c.IsActive,
                c.TenantStatus,
                c.IsDeleted,
                c.PlanId,
                SubscriptionPlanName = c.Plan != null ? c.Plan.Name : null
            })
            .ToListAsync(cancellationToken);

        var ids = companyRows.Select(c => c.Id).ToList();

        var subsRaw = await _db.Subscriptions.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(s => ids.Contains(s.CompanyId))
            .Join(_db.SubscriptionPlans.IgnoreQueryFilters(),
                s => s.PlanId,
                p => p.Id,
                (s, p) => new { s.CompanyId, s.CreatedAt, PlanName = p.Name, p.MaxUsers, p.MaxElevators })
            .ToListAsync(cancellationToken);

        var subByCompany = subsRaw
            .GroupBy(s => s.CompanyId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(s => s.CreatedAt).First());

        var items = companyRows.Select(c =>
        {
            var latestSub = subByCompany.GetValueOrDefault(c.Id);
            return new CompanyListDto(
                c.Id,
                c.Name,
                c.IsActive,
                null,
                c.BillingContactEmail,
                c.ContactPhone,
                PlatformAdminMapper.ToCompanyStatusString(c.TenantStatus, c.IsActive, c.IsDeleted),
                latestSub?.PlanName,
                c.SubscriptionPlanName,
                c.PlanId,
                0,
                latestSub?.MaxUsers ?? 0,
                0,
                latestSub?.MaxElevators ?? 0,
                c.CreatedAt);
        }).ToList();

        return Result<PaginatedResultDto<CompanyListDto>>.Success(
            new PaginatedResultDto<CompanyListDto>(items, total, page, pageSize));
    }
}
