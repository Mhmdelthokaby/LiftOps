using LiftOps_BackEnd.Application.DTOs;
using LiftOps_BackEnd.Application.Features.Inventory.Commands.AddInventoryItem;
using LiftOps_BackEnd.Application.Features.Inventory.Commands.DisableInventoryItem;
using LiftOps_BackEnd.Application.Features.Inventory.Commands.UpdateInventoryItem;
using LiftOps_BackEnd.Application.Features.Inventory.Queries.GetActiveInventoryItems;
using LiftOps_BackEnd.Application.Features.Inventory.Queries.GetAllInventoryItems;
using LiftOps_BackEnd.Application.Features.Inventory.Queries.GetInventoryTotalValue;
using LiftOps_BackEnd.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin)]
    public async Task<IActionResult> Add([FromBody] CreateInventoryItemDto dto)
    {
        var result = await _mediator.Send(new AddInventoryItemCommand(dto));
        if (result == null) return BadRequest(new { Message = "Failed to add inventory item." });
        return Ok(new { Id = result, Message = "Item added successfully." });
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInventoryItemDto dto)
    {
        var result = await _mediator.Send(new UpdateInventoryItemCommand(id, dto));
        if (!result) return BadRequest(new { Message = "Update failed or item not found." });
        return Ok(new { Message = "Item updated successfully." });
    }

    [HttpPut("disable/{id}")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin)]
    public async Task<IActionResult> Disable(Guid id, [FromBody] bool disable)
    {
        var result = await _mediator.Send(new DisableInventoryItemCommand(id, disable));
        if (!result) return BadRequest(new { Message = "Action failed or item not found." });
        return Ok(new { Message = $"Item {(disable ? "disabled" : "enabled")} successfully." });
    }

    [HttpGet("all")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllInventoryItemsQuery());
        return Ok(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin + "," + Roles.InstallationAdmin + "," + Roles.MaintenanceAdmin)]
    public async Task<IActionResult> GetActive()
    {
        var result = await _mediator.Send(new GetActiveInventoryItemsQuery());
        return Ok(result);
    }

    [HttpGet("value")]
    public async Task<IActionResult> GetTotalValue()
    {
        var result = await _mediator.Send(new GetInventoryTotalValueQuery());
        return Ok(new { TotalValue = result });
    }
}
