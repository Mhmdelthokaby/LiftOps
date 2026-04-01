using LiftOps_BackEnd.Application.Features.Admins.Commands.AssignRoles;
using LiftOps_BackEnd.Application.Features.Admins.Commands.DisableAdmin;
using LiftOps_BackEnd.Application.Features.Admins.Commands.LoginAdmin;
using LiftOps_BackEnd.Application.Features.Admins.Commands.RefreshToken;
using LiftOps_BackEnd.Application.Features.Admins.Commands.RegisterAdmin;
using LiftOps_BackEnd.Application.Features.Admins.Commands.UpdateAdmin;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Application.Features.Admins.Queries.ListAdmins;
using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.API.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _mediator.Send(new LoginAdminCommand(loginDto));
        if (result.Auth == null)
        {
            if (result.ErrorCode == "company_membership_required")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                });
            }

            return Unauthorized(new
            {
                code = result.ErrorCode ?? "invalid_credentials",
                message = result.ErrorMessage ?? "Invalid email or password, or account disabled."
            });
        }
        
        return Ok(new 
        {
            result.Auth.Token,
            result.Auth.RefreshToken,
            result.Auth.RefreshTokenExpiry,
            result.Auth.Name,
            result.Auth.Email,
            result.Auth.Roles
        });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.Token, request.RefreshToken));
        if (result.Auth == null)
        {
            if (result.ErrorCode == "company_membership_required")
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = result.ErrorCode,
                    message = result.ErrorMessage
                });
            }

            return Unauthorized(new
            {
                code = result.ErrorCode ?? "invalid_token",
                message = result.ErrorMessage ?? "Invalid or expired token."
            });
        }
        return Ok(result.Auth);
    }

    [HttpPost("register")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> Register([FromBody] RegisterAdminDto registerDto)
    {
        var result = await _mediator.Send(new RegisterAdminCommand(registerDto));
        if (result is Result res && !res.Succeeded)
        {
            return BadRequest(new { Message = "Registration failed.", Errors = res.Errors });
        }
        return Ok(new { Message = "Admin registered successfully." });
    }

    [HttpGet("list")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> List([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort)
    {
        if (!PaginationExtensions.TryNormalize(new PageRequest(page, pageSize, sort), out var p, out var ps, out var s, out var error))
        {
            return BadRequest(new { code = "invalid_pagination", message = error });
        }

        var result = await _mediator.Send(new ListAdminsQuery());
        var ordered = s.Equals("name_desc", StringComparison.OrdinalIgnoreCase)
            ? result.OrderByDescending(x => x.Name)
            : result.OrderBy(x => x.Name);
        var items = ordered.ApplyPaging(p, ps);

        return Ok(new PagedResponse<AdminListItemDto>(p, ps, result.Count, s, items));
    }

    [HttpPut("update/{id}")]
    [Authorize] // Can be Manager or the user themselves (handled by policy or internal check)
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdminDto updateDto)
    {
        // Simple manager or self check
        var isAdmin = User.IsInRole(Roles.Manager);
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!isAdmin && currentUserId != id.ToString())
        {
            return Forbid();
        }

        var result = await _mediator.Send(new UpdateAdminCommand(id, updateDto));
        if (!result) return BadRequest(new { Message = "Update failed." });
        return Ok(new { Message = "Admin updated successfully." });
    }

    [HttpPut("disable/{id}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> Disable(Guid id, [FromBody] bool disable)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == id.ToString())
        {
            return BadRequest(new { Message = "Manager cannot disable their own account." });
        }

        var result = await _mediator.Send(new DisableAdminCommand(id, disable));
        if (!result) return BadRequest(new { Message = "Action failed." });
        return Ok(new { Message = $"Admin {(disable ? "disabled" : "enabled")} successfully." });
    }

    [HttpPut("roles/{id}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] List<string> roles)
    {
        var result = await _mediator.Send(new AssignRolesCommand(id, roles));
        if (!result) return BadRequest(new { Message = "Role assignment failed." });
        return Ok(new { Message = "Roles assigned successfully." });
    }
}

public record RefreshTokenRequest(string Token, string RefreshToken);
