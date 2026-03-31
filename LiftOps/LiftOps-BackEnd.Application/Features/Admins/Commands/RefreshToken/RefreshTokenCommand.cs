using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthCommandResult>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthCommandResult>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthCommandResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        
        if (principal == null) return new AuthCommandResult(null, "invalid_token", "Invalid or expired token.");

        var email = principal.FindFirstValue(ClaimTypes.Email);

        if (email == null) return new AuthCommandResult(null, "invalid_token", "Invalid or expired token.");

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow || user.IsDisabled)
        {
            return new AuthCommandResult(null, "invalid_token", "Invalid or expired token.");
        }

        if (user.CompanyId == Guid.Empty)
        {
            return new AuthCommandResult(null, "company_membership_required", "User is not linked to a company yet.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _tokenService.CreateToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new AuthCommandResult(new AuthResponseDto(
            newToken,
            newRefreshToken,
            user.RefreshTokenExpiry.Value,
            user.FullName,
            user.Email!,
            roles
        ));
    }
}
