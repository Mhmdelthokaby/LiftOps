using LiftOps_BackEnd.Domain.Entities.Subscription;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.API.Middleware;

public class SubscriptionStatusMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> MutatingMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    public SubscriptionStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (!MutatingMethods.Contains(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;
        if (IsAllowlisted(path))
        {
            await _next(context);
            return;
        }

        var rawCompanyId = context.User.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(rawCompanyId, out var companyId))
        {
            await _next(context);
            return;
        }

        using var _ = dbContext.UseSystemTenantBypass(companyId);
        var status = await dbContext.Subscriptions
            .Where(s => s.CompanyId == companyId)
            .Select(s => s.Status)
            .FirstOrDefaultAsync();

        if (status is SubscriptionStatus.Cancelled or SubscriptionStatus.PastDue)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                code = "subscription_inactive",
                message = "Your subscription is inactive. Billing action is required."
            });
            return;
        }

        await _next(context);
    }

    private static bool IsAllowlisted(string path)
    {
        if (path.Equals("/api/billing/portal", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (path.StartsWith("/api/subscription/webhook", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (path.StartsWith("/api/platform", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
