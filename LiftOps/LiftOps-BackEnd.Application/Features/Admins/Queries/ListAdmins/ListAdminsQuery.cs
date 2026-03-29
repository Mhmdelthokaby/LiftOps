using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.Admins.Queries.ListAdmins;

public record ListAdminsQuery() : IRequest<List<AdminListItemDto>>;

public class ListAdminsQueryHandler : IRequestHandler<ListAdminsQuery, List<AdminListItemDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public ListAdminsQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AdminListItemDto>> Handle(ListAdminsQuery request, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
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
