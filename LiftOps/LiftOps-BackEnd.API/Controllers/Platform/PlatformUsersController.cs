using LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers.Platform;

[ApiController]
[Route("api/platform/users")]
[Authorize(Policy = "RequirePlatformAdmin")]
public class PlatformUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlatformUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? search,
        [FromQuery] Guid? companyId,
        [FromQuery] string? role,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetGlobalUsersQuery(page ?? 1, pageSize ?? 20, search, companyId, role),
            cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "users_failed", message = result.Errors.FirstOrDefault() });
        }

        return Ok(result.Data);
    }

    [HttpPost("impersonate")]
    public async Task<IActionResult> Impersonate([FromBody] ImpersonateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ImpersonateUserCommand(request), cancellationToken);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { code = "impersonate_failed", message = result.Errors.FirstOrDefault() ?? "Impersonation failed." });
        }

        var d = result.Data;
        return Ok(new
        {
            token = d.Token,
            refreshToken = d.RefreshToken,
            refreshTokenExpiry = d.RefreshTokenExpiry,
            name = d.Name,
            email = d.Email,
            roles = d.Roles
        });
    }
}
