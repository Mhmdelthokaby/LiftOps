using Collins_BackEnd.Domain.Entities;
using System.Security.Claims;

namespace Collins_BackEnd.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}
