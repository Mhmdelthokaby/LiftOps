using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetGlobalUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? CompanyId = null,
    string? Role = null) : IRequest<Result<PaginatedResultDto<GlobalUserDto>>>;

public class GetGlobalUsersQueryHandler : IRequestHandler<GetGlobalUsersQuery, Result<PaginatedResultDto<GlobalUserDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public GetGlobalUsersQueryHandler(IApplicationDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<Result<PaginatedResultDto<GlobalUserDto>>> Handle(GetGlobalUsersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        IQueryable<AppUser> q = _db.Users.IgnoreQueryFilters().AsNoTracking();

        if (request.CompanyId.HasValue)
        {
            var cid = request.CompanyId.Value;
            q = q.Where(u => u.CompanyId == cid);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(u =>
                (u.Email != null && u.Email.Contains(s)) ||
                u.FullName.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var normalized = request.Role.Trim().ToUpperInvariant();
            var roleId = await _db.Roles
                .AsNoTracking()
                .Where(r => r.NormalizedName == normalized)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (roleId == Guid.Empty)
            {
                return Result<PaginatedResultDto<GlobalUserDto>>.Success(
                    new PaginatedResultDto<GlobalUserDto>(Array.Empty<GlobalUserDto>(), 0, page, pageSize));
            }

            q = from u in q
                join ur in _db.UserRoles.AsNoTracking() on u.Id equals ur.UserId
                where ur.RoleId == roleId
                select u;
            q = q.Distinct();
        }

        var total = await q.CountAsync(cancellationToken);

        var users = await q
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var companyIds = users.Where(u => u.CompanyId.HasValue).Select(u => u.CompanyId!.Value).Distinct().ToList();
        var companies = await _db.Companies.IgnoreQueryFilters()
            .AsNoTracking()
            .Where(c => companyIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var items = new List<GlobalUserDto>();
        foreach (var u in users)
        {
            PlatformAdminMapper.SplitFullName(u.FullName, out var first, out var last);
            var companyName = u.CompanyId is { } cid && companies.TryGetValue(cid, out var n) ? n : string.Empty;
            var roleList = await _userManager.GetRolesAsync(u);
            var primaryRole = roleList.FirstOrDefault() ?? string.Empty;

            items.Add(new GlobalUserDto(
                u.Id,
                u.Email ?? string.Empty,
                first,
                last,
                u.CompanyId,
                companyName,
                primaryRole,
                u.LastLogin,
                !u.IsDisabled));
        }

        return Result<PaginatedResultDto<GlobalUserDto>>.Success(
            new PaginatedResultDto<GlobalUserDto>(items, total, page, pageSize));
    }
}
