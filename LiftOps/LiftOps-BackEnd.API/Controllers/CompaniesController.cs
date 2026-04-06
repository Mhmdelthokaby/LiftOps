using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/companies")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCompanyCommand(id, request), cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { code = "update_failed", message = result.Errors.FirstOrDefault() ?? "Update failed." });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCompanyCommand(id), cancellationToken);
        if (!result.Succeeded)
        {
            return BadRequest(new { code = "delete_failed", message = result.Errors.FirstOrDefault() ?? "Delete failed." });
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCompanyCommand(request), cancellationToken);
        
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new 
            { 
                code = "create_company_failed", 
                message = result.Errors.FirstOrDefault() ?? "Failed to create company." 
            });
        }

        // Return the created company details and initial password as requested
        return Ok(new
        {
            company = result.Data.Company,
            initialPassword = result.Data.InitialPassword
        });
    }

    [HttpGet("profile")]
    [Authorize(Policy = "RequireManager")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var rawCompanyId = User.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(rawCompanyId, out var companyId))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new GetCompanyByIdQuery(companyId), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = result.Data.Id,
            name = result.Data.Name,
            contactEmail = result.Data.ContactEmail,
            contactPhone = result.Data.ContactPhone,
            planName = result.Data.SubscriptionPlan,
            planId = result.Data.PlanId
        });
    }

    [HttpPut("profile")]
    [Authorize(Policy = "RequireManager")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCompanyProfileDto request, CancellationToken cancellationToken)
    {
        var rawCompanyId = User.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(rawCompanyId, out var companyId))
        {
            return Unauthorized();
        }

        // We use a simplified update for the tenant, ensuring they can't change their plan or isActive status
        var result = await _mediator.Send(new UpdateCompanyCommand(companyId, new UpdateCompanyRequest 
        { 
            Name = request.Name,
            IsActive = true, // Force true to prevent self-deactivation
            PlanId = null // Passing null ensures the handler doesn't change it
        }), cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(new { code = "update_failed", message = result.Errors.FirstOrDefault() ?? "Update failed." });
        }

        return NoContent();
    }
}

public record UpdateCompanyProfileDto(string Name, string ContactEmail, string? ContactPhone, string? Address, string? City);
