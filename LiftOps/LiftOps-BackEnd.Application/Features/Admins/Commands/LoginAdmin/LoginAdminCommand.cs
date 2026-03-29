using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.LoginAdmin;

public record LoginAdminCommand(LoginDto LoginDto) : IRequest<AuthResponseDto?>;

public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, AuthResponseDto?>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginAdminCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.LoginDto.Email);

        if (user == null || user.IsDisabled) return null;

        var result = await _userManager.CheckPasswordAsync(user, request.LoginDto.Password);

        if (!result) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.CreateToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.LastLogin = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return new AuthResponseDto(
            token,
            refreshToken,
            user.RefreshTokenExpiry.Value,
            user.FullName,
            user.Email!,
            roles
        );
    }
}
