using System.Security.Claims;
using LiftOps_BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LiftOps_BackEnd.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CompanyId
    {
        get
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            var rawCompanyId = principal?.FindFirstValue("company_id")
                ?? principal?.FindFirstValue("tenant_id");

            return Guid.TryParse(rawCompanyId, out var companyId) ? companyId : null;
        }
    }

    public string? Slug
    {
        get
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return principal?.FindFirstValue("company_slug")
                ?? principal?.FindFirstValue("tenant_slug");
        }
    }

    public bool IsResolved
    {
        get
        {
            var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
            return isAuthenticated && (CompanyId.HasValue || !string.IsNullOrWhiteSpace(Slug));
        }
    }
}
