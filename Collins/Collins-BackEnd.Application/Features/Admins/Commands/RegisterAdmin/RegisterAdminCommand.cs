using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Application.Features.Admins.DTOs;
using Collins_BackEnd.Domain.Common;
using Collins_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Collins_BackEnd.Application.Features.Admins.Commands.RegisterAdmin;

public record RegisterAdminCommand(RegisterAdminDto RegisterDto) : IRequest<Result>;

public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RegisterAdminCommandHandler(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.RegisterDto.Email);
        if (existingUser != null) return Result.Failure("A user with this email already exists.");

        var user = new AppUser
        {
            FullName = request.RegisterDto.Name,
            Email = request.RegisterDto.Email,
            UserName = request.RegisterDto.Email,
            PhoneNumber = request.RegisterDto.Phone,
            IsDisabled = false
        };

        var result = await _userManager.CreateAsync(user, request.RegisterDto.Password);

        if (result.Succeeded)
        {
            if (request.RegisterDto.Roles != null && request.RegisterDto.Roles.Any())
            {
                // Ensure each role exists; create if missing
                foreach (var role in request.RegisterDto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                }
                await _userManager.AddToRolesAsync(user, request.RegisterDto.Roles);
            }
            return Result.Success();
        }

        return Result.Failure(result.Errors.Select(e => e.Description).ToArray());
    }
}
