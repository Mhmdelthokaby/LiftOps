using Collins_BackEnd.Application.Features.Installation.Commands;
using Collins_BackEnd.Application.Features.Installation.DTOs;
using Collins_BackEnd.Application.Features.Installation.Queries.GetAllCustomers;
using Collins_BackEnd.Domain.Common;
using Collins_BackEnd.Domain.Entities.Installation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Manager + "," + Roles.InstallationAdmin)] // Only Manager and InstallationAdmin can access customers
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        var result = await _mediator.Send(new UpdateCustomerCommand(id, dto));
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Failed to update customer", Errors = result.Errors });
        }
        return Ok(new { Message = "Customer updated successfully", Id = result.Data });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CustomerStatus status)
    {
        var result = await _mediator.Send(new UpdateCustomerStatusCommand(id, status));
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Failed to update customer status", Errors = result.Errors });
        }
        return Ok(new { Message = "Customer status updated successfully", Id = result.Data });
    }
}
