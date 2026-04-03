using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers.Platform;

[ApiController]
[Route("api/platform/dashboard")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class PlatformDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlatformDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPlatformDashboardQuery(), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "dashboard_failed", message = result.Errors.FirstOrDefault() ?? "Failed to load dashboard." });
        }

        return Ok(result.Data);
    }
}
