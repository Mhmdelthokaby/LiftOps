using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthResponseDto?>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto?>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        
        if (principal == null) return null;

        var email = principal.FindFirstValue(ClaimTypes.Email);

        if (email == null) return null;

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow || user.IsDisabled)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _tokenService.CreateToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto(
            newToken,
            newRefreshToken,
            user.RefreshTokenExpiry.Value,
            user.FullName,
            user.Email!,
            roles
        );
    }
}
