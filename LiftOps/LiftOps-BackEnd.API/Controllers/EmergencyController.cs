using LiftOps_BackEnd.Application.DTOs.Emergency;
using LiftOps_BackEnd.Application.Features.Emergency.Commands;
using LiftOps_BackEnd.Application.Features.Emergency.Queries;
using LiftOps_BackEnd.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmergencyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmergencyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateEmergencyTicketDto dto)
        {
            var result = await _mediator.Send(new CreateEmergencyTicketCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var result = await _mediator.Send(new GetAllEmergencyTicketsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(Guid id)
        {
            var result = await _mediator.Send(new GetEmergencyTicketByIdQuery { TicketId = id });
            if (!result.Succeeded) return NotFound(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateEmergencyTicketDto dto)
        {
            var result = await _mediator.Send(new UpdateEmergencyTicketCommand { TicketId = id, Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var result = await _mediator.Send(new DeleteEmergencyTicketCommand { TicketId = id });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/assign-technician")]
        public async Task<IActionResult> AssignTechnician(Guid id, [FromBody] AssignEmergencyTechnicianDto dto)
        {
            var result = await _mediator.Send(new AssignEmergencyTechnicianCommand { TicketId = id, Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(Guid id, [FromBody] ResolveEmergencyTicketRequest request)
        {
            var command = new ResolveEmergencyTicketCommand { TicketId = id, Notes = request.Notes };
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("open")]
        public async Task<IActionResult> GetOpenTickets()
        {
            var result = await _mediator.Send(new GetOpenEmergencyTicketsQuery());
            return Ok(result);
        }
    }
}

