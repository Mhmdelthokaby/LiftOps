using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Application.Features.Admins.DTOs;

public record AuthResponseDto(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiry,
    string Name,
    string Email,
    IList<string> Roles
);

public record LoginDto(string Email, string Password);

public record RegisterAdminDto(
    string Name,
    string Email,
    string Phone,
    string Password,
    List<string> Roles,
    Guid? CompanyId = null,
    string? CompanySlug = null,
    string? CompanyName = null
);

public record UpdateAdminDto(
    string Name,
    string Email,
    string Phone,
    string? Password
);

public record AdminListItemDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    IList<string> Roles,
    bool IsDisabled,
    DateTime? LastLogin
);
