using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LiftOps_BackEnd.API.Filters;

/// <summary>
/// Returns 404 when the host environment is not Development so probe endpoints are not exposed in staging/production.
/// </summary>
public sealed class DevelopmentOnlyFilter : IAsyncActionFilter
{
    private readonly IWebHostEnvironment _environment;

    public DevelopmentOnlyFilter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_environment.IsDevelopment())
        {
            context.Result = new NotFoundResult();
            return Task.CompletedTask;
        }

        return next();
    }
}
