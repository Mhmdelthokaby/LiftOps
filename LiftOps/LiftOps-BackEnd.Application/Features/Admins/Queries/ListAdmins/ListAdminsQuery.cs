using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.Admins.Queries.ListAdmins;

public record ListAdminsQuery() : IRequest<List<AdminListItemDto>>;

public class ListAdminsQueryHandler : IRequestHandler<ListAdminsQuery, List<AdminListItemDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentTenantService _currentTenantService;

    public ListAdminsQueryHandler(UserManager<AppUser> userManager, ICurrentTenantService currentTenantService)
    {
        _userManager = userManager;
        _currentTenantService = currentTenantService;
    }

    public async Task<List<AdminListItemDto>> Handle(ListAdminsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentTenantService.CompanyId.HasValue)
        {
            return new List<AdminListItemDto>();
        }

        var tenantId = _currentTenantService.CompanyId.Value;
        var users = await _userManager.Users
            .Where(u => u.CompanyId == tenantId)
            .ToListAsync(cancellationToken);
        var list = new List<AdminListItemDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            list.Add(new AdminListItemDto(
                user.Id,
                user.FullName,
                user.Email!,
                user.PhoneNumber!,
                roles,
                user.IsDisabled,
                user.LastLogin
            ));
        }

        return list;
    }
}
