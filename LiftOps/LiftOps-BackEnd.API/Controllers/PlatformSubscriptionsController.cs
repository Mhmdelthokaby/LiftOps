using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/platform/subscriptions")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class PlatformSubscriptionsController : ControllerBase
{
    private readonly ISubscriptionLifecycleService _subscriptionLifecycleService;

    public PlatformSubscriptionsController(ISubscriptionLifecycleService subscriptionLifecycleService)
    {
        _subscriptionLifecycleService = subscriptionLifecycleService;
    }

    [HttpPost("{companyId:guid}/extend-trial")]
    public async Task<IActionResult> ExtendTrial(Guid companyId, [FromBody] ExtendTrialRequest request, CancellationToken cancellationToken)
    {
        if (request.Days <= 0)
        {
            return BadRequest(new { code = "invalid_trial_extension_days", message = "Days must be greater than zero." });
        }

        var updated = await _subscriptionLifecycleService.ExtendTrialAsync(companyId, request.Days, cancellationToken);
        if (!updated)
        {
            return NotFound(new { code = "subscription_not_found", message = "Subscription not found for company." });
        }

        return Ok(new { message = "Trial period extended successfully." });
    }

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

public record ExtendTrialRequest(int Days);
public record SetSubscriptionStatusRequest(SubscriptionStatus Status);
