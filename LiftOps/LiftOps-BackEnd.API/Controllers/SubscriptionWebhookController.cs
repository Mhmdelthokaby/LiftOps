using LiftOps_BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/subscription/webhook")]
public class SubscriptionWebhookController : ControllerBase
{
    private readonly ISubscriptionLifecycleService _subscriptionLifecycleService;
    private readonly IConfiguration _configuration;

    public SubscriptionWebhookController(
        ISubscriptionLifecycleService subscriptionLifecycleService,
        IConfiguration configuration)
    {
        _subscriptionLifecycleService = subscriptionLifecycleService;
        _configuration = configuration;
    }

    [HttpPost("payment-failed")]
    [AllowAnonymous]
    public async Task<IActionResult> PaymentFailed([FromBody] PaymentFailedWebhookRequest request, CancellationToken cancellationToken)
    {
        var configuredSecret = _configuration["Subscription:WebhookSecret"];
        if (!string.IsNullOrWhiteSpace(configuredSecret))
        {
            var providedSecret = Request.Headers["X-Webhook-Secret"].ToString();
            if (!string.Equals(providedSecret, configuredSecret, StringComparison.Ordinal))
            {
                return Unauthorized(new { code = "invalid_webhook_signature", message = "Invalid webhook secret." });
            }
        }

        var updated = await _subscriptionLifecycleService.MarkPastDueAsync(request.CompanyId, request.ExternalCustomerId, cancellationToken);
        if (!updated)
        {
            return NotFound(new { code = "subscription_not_found", message = "Subscription not found for company." });
        }

        return Ok(new { message = "Subscription marked as PastDue." });
    }
}

public record PaymentFailedWebhookRequest(Guid CompanyId, string? ExternalCustomerId);
