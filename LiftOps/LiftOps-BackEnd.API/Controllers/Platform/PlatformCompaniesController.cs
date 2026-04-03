using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers.Platform;

[ApiController]
[Route("api/platform/companies")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class PlatformCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlatformCompaniesController(IMediator mediator)
    {
        _mediator = mediator;
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
            new GetCompaniesQuery(page ?? 1, pageSize ?? 20, search, status, planId),
            cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "list_failed", message = result.Errors.FirstOrDefault() });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound(new { code = "company_not_found", message = result.Errors.FirstOrDefault() ?? "Company not found." });
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCompanyCommand(request), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "create_company_failed", message = result.Errors.FirstOrDefault() ?? "Failed to create company." });
        }

        var c = result.Data.Company;
        var sub = c.Subscription;
        return Ok(new
        {
            id = c.Id,
            name = c.Name,
            contactEmail = c.ContactEmail,
            status = c.Status,
            createdAt = c.CreatedAt,
            currentUserCount = 1,
            currentElevatorCount = 0,
            subscription = sub == null
                ? null
                : new
                {
                    id = sub.Id,
                    companyId = sub.CompanyId,
                    planId = sub.PlanId,
                    status = sub.Status,
                    currentPeriodStart = sub.CurrentPeriodStart,
                    currentPeriodEnd = sub.CurrentPeriodEnd,
                    billingCycle = sub.BillingCycle,
                    planName = sub.PlanName
                },
            planMaxUsers = c.PlanLimits?.MaxUsers,
            planMaxElevators = c.PlanLimits?.MaxElevators,
            initialPassword = result.Data.InitialPassword
        });
    }

    [HttpPut("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, [FromBody] SuspendCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SuspendCompanyCommand(id, request), cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { code = "suspend_failed", message = result.Errors.FirstOrDefault() });
        }

        return NoContent();
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ActivateCompanyCommand(id), cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { code = "activate_failed", message = result.Errors.FirstOrDefault() });
        }

        return NoContent();
    }
}
