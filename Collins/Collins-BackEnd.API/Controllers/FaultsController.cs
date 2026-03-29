using Collins_BackEnd.Application.DTOs.Faults;
using Collins_BackEnd.Application.Features.Faults.Commands;
using Collins_BackEnd.Application.Features.Faults.Queries;
using Collins_BackEnd.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Collins_BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaultsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FaultsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-ticket")]
        // Accessible by anyone? Or just admins/users? Usually public or authenticated.
        // Assuming Admin allows entry, or maybe customers? 
        // Docs say "FaultsAdmin OR MaintenanceAdmin". But maybe public reporting?
        // I'll secure it for now as per roles.
        [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin + "," + Roles.FaultsAdmin)]
        public async Task<IActionResult> CreateTicket([FromBody] CreateFaultTicketDto dto)
        {
            var result = await _mediator.Send(new CreateFaultTicketCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{ticketId}/assign-technician")]
        [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin + "," + Roles.FaultsAdmin)]
        public async Task<IActionResult> AssignTechnician(Guid ticketId, [FromBody] AssignFaultTechnicianDto dto)
        {
            var result = await _mediator.Send(new AssignFaultTechnicianCommand { TicketId = ticketId, Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{ticketId}/resolve")]
        [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin + "," + Roles.FaultsAdmin)]
        public async Task<IActionResult> ResolveTicket(Guid ticketId, [FromBody] ResolveFaultDto dto)
        {
            var result = await _mediator.Send(new ResolveFaultCommand { TicketId = ticketId, Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("open")]
        [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin + "," + Roles.FaultsAdmin)]
        public async Task<IActionResult> GetOpenTickets()
        {
            var result = await _mediator.Send(new GetOpenFaultsQuery());
            return Ok(result);
        }
    }
}
