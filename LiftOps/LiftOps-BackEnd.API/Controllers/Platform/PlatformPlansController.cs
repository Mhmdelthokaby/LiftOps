using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers.Platform;

[ApiController]
[Route("api/platform/plans")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class PlatformPlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlatformPlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPlansQuery(), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "plans_failed", message = result.Errors.FirstOrDefault() });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPlanByIdQuery(id), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound(new { code = "plan_not_found", message = result.Errors.FirstOrDefault() ?? "Plan not found." });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreatePlanCommand(request), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "create_plan_failed", message = result.Errors.FirstOrDefault() ?? "Failed to create plan." });
        }

        return Ok(result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdatePlanCommand(id, request), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "update_plan_failed", message = result.Errors.FirstOrDefault() ?? "Failed to update plan." });
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePlanCommand(id), cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { code = "delete_plan_failed", message = result.Errors.FirstOrDefault() });
        }

        return NoContent();
    }
}
