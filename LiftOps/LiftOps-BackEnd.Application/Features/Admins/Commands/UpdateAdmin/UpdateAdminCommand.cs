using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.UpdateAdmin;

public record UpdateAdminCommand(Guid Id, UpdateAdminDto UpdateDto) : IRequest<bool>;

public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;

    public UpdateAdminCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return false;

        user.FullName = request.UpdateDto.Name;
        user.Email = request.UpdateDto.Email;
        user.UserName = request.UpdateDto.Email;
        user.PhoneNumber = request.UpdateDto.Phone;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded && !string.IsNullOrEmpty(request.UpdateDto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, request.UpdateDto.Password);
        }

        return result.Succeeded;
    }
}
