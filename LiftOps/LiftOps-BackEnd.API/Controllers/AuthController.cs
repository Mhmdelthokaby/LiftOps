using LiftOps_BackEnd.Application.Features.Admins.Commands.LoginAdmin;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

/// <summary>Canonical REST path <c>POST /api/auth/login</c> (same behavior as <c>/api/Admin/login</c>).</summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
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
}
