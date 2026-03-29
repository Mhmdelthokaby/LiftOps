using LiftOps_BackEnd.Application.DTOs;
using LiftOps_BackEnd.Application.Features.Categories.Commands.AddCategory;
using LiftOps_BackEnd.Application.Features.Categories.Queries.ListCategories;
using LiftOps_BackEnd.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiftOps_BackEnd.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin)]
    public async Task<IActionResult> Add([FromBody] CreateCategoryDto dto)
    {
        var result = await _mediator.Send(new AddCategoryCommand(dto));
        if (result == null) return BadRequest(new { Message = "Failed to add category." });
        return Ok(new { Id = result, Message = "Category added successfully." });
    }

    [HttpGet("list")]
    [Authorize(Roles = Roles.Manager + "," + Roles.InventoryAdmin + "," + Roles.InstallationAdmin + "," + Roles.MaintenanceAdmin)]
    public async Task<IActionResult> List()
    {
        var result = await _mediator.Send(new ListCategoriesQuery());
        return Ok(result);
    }
}
