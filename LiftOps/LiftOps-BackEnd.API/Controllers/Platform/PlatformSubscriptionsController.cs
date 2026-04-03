using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers.Platform;

[ApiController]
[Route("api/platform/subscriptions")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class PlatformSubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISubscriptionLifecycleService _subscriptionLifecycleService;

    public PlatformSubscriptionsController(
        IMediator mediator,
        ISubscriptionLifecycleService subscriptionLifecycleService)
    {
        _mediator = mediator;
        _subscriptionLifecycleService = subscriptionLifecycleService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? planId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubscriptionsQuery(page ?? 1, pageSize ?? 20, search, status, planId),
            cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "subscriptions_failed", message = result.Errors.FirstOrDefault() });
        }

        return Ok(result.Data);
    }

    /// <summary>Extends trial when subscription is in Trial status (strict). Frontend-compatible route.</summary>
    [HttpPost("{companyId:guid}/extend-trial")]
    public async Task<IActionResult> ExtendTrial(Guid companyId, [FromBody] ExtendTrialRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new ExtendTrialCommand(companyId, request), cancellationToken);
            if (!result.Succeeded)
            {
                return NotFound(new { code = "subscription_not_found", message = result.Errors.FirstOrDefault() ?? "Subscription not found for company." });
            }

            return NoContent();
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { code = "validation_failed", message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? ex.Message });
        }
    }

    [HttpPut("{companyId:guid}/change-plan")]
    public async Task<IActionResult> ChangePlan(Guid companyId, [FromBody] ChangePlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new ChangePlanCommand(companyId, request), cancellationToken);
            if (!result.Succeeded)
            {
                return BadRequest(new { code = "change_plan_failed", message = result.Errors.FirstOrDefault() });
            }

            return NoContent();
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { code = "validation_failed", message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? ex.Message });
        }
    }

    /// <summary>Legacy endpoint for subscription status updates (webhooks / ops).</summary>
    [HttpPut("{companyId:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid companyId, [FromBody] SetSubscriptionStatusRequest request, CancellationToken cancellationToken)
    {
        var updated = await _subscriptionLifecycleService.SetStatusAsync(companyId, request.Status, cancellationToken);
        if (!updated)
        {
            return NotFound(new { code = "subscription_not_found", message = "Subscription not found for company." });
        }

        return Ok(new { message = "Subscription status updated successfully." });
    }
}

public record SetSubscriptionStatusRequest(SubscriptionStatus Status);
