using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.AssignRoles;

public record AssignRolesCommand(Guid Id, List<string> Roles) : IRequest<bool>;

public class AssignRolesCommandHandler : IRequestHandler<AssignRolesCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentTenantService _currentTenantService;

    public AssignRolesCommandHandler(UserManager<AppUser> userManager, ICurrentTenantService currentTenantService)
    {
        _userManager = userManager;
        _currentTenantService = currentTenantService;
    }

    public async Task<bool> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return false;
        if (!_currentTenantService.CompanyId.HasValue || user.CompanyId != _currentTenantService.CompanyId.Value) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRolesAsync(user, request.Roles);

        return result.Succeeded;
    }
}
