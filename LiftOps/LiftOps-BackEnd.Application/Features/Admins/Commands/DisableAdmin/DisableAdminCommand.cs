using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.DisableAdmin;

public record DisableAdminCommand(Guid Id, bool Disable) : IRequest<bool>;

public class DisableAdminCommandHandler : IRequestHandler<DisableAdminCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentTenantService _currentTenantService;

    public DisableAdminCommandHandler(UserManager<AppUser> userManager, ICurrentTenantService currentTenantService)
    {
        _userManager = userManager;
        _currentTenantService = currentTenantService;
    }

    public async Task<bool> Handle(DisableAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return false;
        if (!_currentTenantService.CompanyId.HasValue || user.CompanyId != _currentTenantService.CompanyId.Value) return false;

        user.IsDisabled = request.Disable;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }
}
