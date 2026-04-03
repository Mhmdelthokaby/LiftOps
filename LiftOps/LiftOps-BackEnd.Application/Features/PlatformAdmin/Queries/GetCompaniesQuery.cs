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
                "Active" => q.Where(c => c.TenantStatus == CompanyTenantStatus.Active && c.IsActive),
                "Suspended" => q.Where(c => c.TenantStatus == CompanyTenantStatus.Suspended || (!c.IsActive && c.TenantStatus == CompanyTenantStatus.Active)),
                "SuspendedByAdmin" => q.Where(c => c.TenantStatus == CompanyTenantStatus.SuspendedByAdmin),
                "Deleted" => q.Where(c => c.TenantStatus == CompanyTenantStatus.Deleted),
                _ => q
            };
        }

        var total = await q.CountAsync(cancellationToken);

        var companyRows = await q
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new { c.Id, c.Name, c.BillingContactEmail, c.CreatedAt, c.IsActive, c.TenantStatus })
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
            .GroupBy(x => x.CompanyId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.CreatedAt).First());

        var userCounts = await _db.Users.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(u => ids.Contains(u.CompanyId))
            .GroupBy(u => u.CompanyId)
            .Select(g => new { CompanyId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        var userCountDict = userCounts.ToDictionary(x => x.CompanyId, x => x.Count);

        var elevatorCounts = await _db.Elevators.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(e => ids.Contains(e.CompanyId))
            .GroupBy(e => e.CompanyId)
            .Select(g => new { CompanyId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        var elevDict = elevatorCounts.ToDictionary(x => x.CompanyId, x => x.Count);

        var maintElevCounts = await _db.MaintenanceElevators.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(me => ids.Contains(me.CompanyId))
            .GroupBy(me => me.CompanyId)
            .Select(g => new { CompanyId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        foreach (var m in maintElevCounts)
        {
            if (elevDict.ContainsKey(m.CompanyId))
            {
                elevDict[m.CompanyId] += m.Count;
            }
            else
            {
                elevDict[m.CompanyId] = m.Count;
            }
        }

        var items = companyRows.Select(c =>
        {
            subByCompany.TryGetValue(c.Id, out var sub);
            userCountDict.TryGetValue(c.Id, out var uc);
            elevDict.TryGetValue(c.Id, out var ec);
            var company = new Company
            {
                Id = c.Id,
                Name = c.Name,
                BillingContactEmail = c.BillingContactEmail,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive,
                TenantStatus = c.TenantStatus
            };
            return new CompanyListDto(
                c.Id,
                c.Name,
                c.BillingContactEmail,
                PlatformAdminMapper.ToCompanyStatusString(company),
                sub?.PlanName,
                uc,
                sub?.MaxUsers ?? 0,
                ec,
                sub?.MaxElevators ?? 0,
                c.CreatedAt);
        }).ToList();

        return Result<PaginatedResultDto<CompanyListDto>>.Success(
            new PaginatedResultDto<CompanyListDto>(items, total, page, pageSize));
    }
}
