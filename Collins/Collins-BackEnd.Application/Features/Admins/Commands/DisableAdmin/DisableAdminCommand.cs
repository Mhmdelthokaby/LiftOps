using Collins_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Collins_BackEnd.Application.Features.Admins.Commands.DisableAdmin;

public record DisableAdminCommand(Guid Id, bool Disable) : IRequest<bool>;

public class DisableAdminCommandHandler : IRequestHandler<DisableAdminCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;

    public DisableAdminCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(DisableAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return false;

        user.IsDisabled = request.Disable;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }
}
