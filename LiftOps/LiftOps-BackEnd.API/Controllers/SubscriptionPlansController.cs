using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/subscription-plans")]
public class SubscriptionPlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionPlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetActivePlansQuery(), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "plans_failed", message = result.Errors.FirstOrDefault() });
        }

        return Ok(result.Data);
    }
}
