using Microsoft.AspNetCore.Http;
using Collins_BackEnd.Application.Interfaces;
using System.Security.Claims;

namespace Collins_BackEnd.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var result) ? result : null;
        }
    }

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
}
