using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record ImpersonateUserCommand(ImpersonateUserRequest Request) : IRequest<Result<ImpersonateUserResponseDto>>;

public class ImpersonateUserCommandHandler : IRequestHandler<ImpersonateUserCommand, Result<ImpersonateUserResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public ImpersonateUserCommandHandler(
        IApplicationDbContext db,
        UserManager<AppUser> userManager,
        ITokenService tokenService)
    {
        _db = db;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<ImpersonateUserResponseDto>> Handle(ImpersonateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == request.Request.UserId, cancellationToken);

        if (user == null || user.IsDisabled)
        {
            return Result<ImpersonateUserResponseDto>.Failure("User not found or disabled.");
        }

        if (!user.CompanyId.HasValue || user.CompanyId == Guid.Empty)
        {
            return Result<ImpersonateUserResponseDto>.Failure("User is not linked to a company.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(Roles.PlatformAdmin, StringComparer.OrdinalIgnoreCase))
        {
            return Result<ImpersonateUserResponseDto>.Failure("Cannot impersonate a platform administrator.");
        }

        var token = _tokenService.CreateToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expiry = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiry;
        await _userManager.UpdateAsync(user);

        var dto = new ImpersonateUserResponseDto(
            token,
            refreshToken,
            expiry,
            user.FullName,
            user.Email ?? string.Empty,
            roles.ToList());

        return Result<ImpersonateUserResponseDto>.Success(dto);
    }
}
