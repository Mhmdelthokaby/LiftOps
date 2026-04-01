using LiftOps_BackEnd.Application.Features.Maintenance.Queries.GetTechnicianVisitsForToday;
using LiftOps_BackEnd.Application.Features.Maintenance.Commands.UpdateVisitStatus;
using LiftOps_BackEnd.Application.Features.Maintenance.Commands;
using LiftOps_BackEnd.Application.Features.Maintenance.Queries.ListChecklistItems;
using LiftOps_BackEnd.Application.Features.Technicians.Commands;
using LiftOps_BackEnd.Application.Features.Technicians.Queries;
using LiftOps_BackEnd.Application.Features.Emergency.Queries;
using LiftOps_BackEnd.API.Common;
using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TechnicianController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly ITechnicianService _technicianService;

        public TechnicianController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            IMaintenanceService maintenanceService,
            ITechnicianService technicianService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _maintenanceService = maintenanceService;
            _technicianService = technicianService;
        }

        // Get technician's assigned visits for today
        [HttpGet("visits/today")]
        public async Task<IActionResult> GetMyVisitsForToday([FromQuery] DateTime? date = null)
        {
            // Get technician ID from current user
            var technicianId = await GetTechnicianIdFromUserAsync();
            if (!technicianId.HasValue)
            {
                return Unauthorized(new { Message = "User is not linked to a technician." });
            }

            var result = await _mediator.Send(new GetTechnicianVisitsForTodayQuery 
            { 
                TechnicianId = technicianId.Value,
                Date = date
            });
            
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get visit details
        [HttpGet("visits/{visitId}")]
        public async Task<IActionResult> GetVisitDetails(Guid visitId)
        {
            var technicianId = await GetTechnicianIdFromUserAsync();
            if (!technicianId.HasValue)
            {
                return Unauthorized(new { Message = "User is not linked to a technician." });
            }

            var visit = await _maintenanceService.GetVisitByIdAsync(visitId);
            if (visit == null) return NotFound(new { Message = "Visit not found." });

            // Verify technician is assigned to this visit
            if (visit.TechnicianId != technicianId.Value)
            {
                return Forbid("You are not assigned to this visit.");
            }

            // Map to DTO
            var visitDto = new TechnicianVisitDto
            {
                VisitId = visit.Id,
                ElevatorId = visit.MaintenanceElevatorId,
                ElevatorCode = visit.MaintenanceElevator != null 
                    ? $"{visit.MaintenanceElevator.Contract?.ProjectNumber ?? "N/A"}-M{visit.MaintenanceElevator.Id.ToString().Substring(0, 8)}"
                    : "N/A",
                ContractId = visit.MaintenanceElevator?.ContractId ?? Guid.Empty,
                ProjectNumber = visit.MaintenanceElevator?.Contract?.ProjectNumber ?? "",
                ProjectName = visit.MaintenanceElevator?.Contract?.ProjectNumber ?? "",
                ProjectNotes = null, // MaintenanceContract doesn't have Notes property
                CustomerId = visit.MaintenanceElevator?.Contract?.CustomerId ?? Guid.Empty,
                CustomerName = visit.MaintenanceElevator?.Contract?.Customer?.Name ?? "",
                CustomerPhone = visit.MaintenanceElevator?.Contract?.Customer?.Phone ?? "",
                CustomerAddress = visit.MaintenanceElevator?.Contract?.ProjectAddress ?? 
                                 visit.MaintenanceElevator?.Contract?.Customer?.Address ?? "",
                City = visit.MaintenanceElevator?.Contract?.City ?? 
                       visit.MaintenanceElevator?.Contract?.Customer?.City ?? "",
                GoogleMapsLink = visit.MaintenanceElevator?.Contract?.GoogleMapsLink,
                VisitDate = visit.VisitDate,
                Status = visit.Status.ToString(),
                Notes = visit.Notes,
                PaymentNotes = visit.PaymentNotes,
                IsPaid = visit.IsPaid,
                CompletedDate = visit.CompletedDate
            };

            return Ok(visitDto);
        }

        // Update visit status (InProgress, Done)
        [HttpPut("visits/{visitId}/status")]
        public async Task<IActionResult> UpdateVisitStatus(Guid visitId, [FromBody] UpdateVisitStatusDto dto)
        {
            var technicianId = await GetTechnicianIdFromUserAsync();
            if (!technicianId.HasValue)
            {
                return Unauthorized(new { Message = "User is not linked to a technician." });
            }

            // Verify technician is assigned to this visit
            var visit = await _maintenanceService.GetVisitByIdAsync(visitId);
            if (visit == null) return NotFound(new { Message = "Visit not found." });
            if (visit.TechnicianId != technicianId.Value)
            {
                return Forbid("You are not assigned to this visit.");
            }

            dto.VisitId = visitId;
            var result = await _mediator.Send(new UpdateVisitStatusCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result.Data);
        }

        // Complete visit with notes, payment, and checklist
        [HttpPost("visits/{visitId}/complete")]
        public async Task<IActionResult> CompleteVisit(Guid visitId, [FromBody] CompleteVisitDto dto)
        {
            var technicianId = await GetTechnicianIdFromUserAsync();
            if (!technicianId.HasValue)
            {
                return Unauthorized(new { Message = "User is not linked to a technician." });
            }

            // Verify technician is assigned to this visit
            var visit = await _maintenanceService.GetVisitByIdAsync(visitId);
            if (visit == null) return NotFound(new { Message = "Visit not found." });
            if (visit.TechnicianId != technicianId.Value)
            {
                return Forbid("You are not assigned to this visit.");
            }

            dto.VisitId = visitId;
            var result = await _mediator.Send(new CompleteVisitCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result.Data);
        }

        // ========== Technician CRUD Endpoints ==========

        [HttpGet("all")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> GetAllTechnicians([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort)
        {
            if (!PaginationExtensions.TryNormalize(new PageRequest(page, pageSize, sort), out var p, out var ps, out var s, out var error))
            {
                return BadRequest(new { code = "invalid_pagination", message = error });
            }

            var result = await _mediator.Send(new GetAllTechniciansQuery());
            if (!result.Succeeded) return BadRequest(result);
            var data = result.Data ?? new List<TechnicianDto>();
            var ordered = s.Equals("name_desc", StringComparison.OrdinalIgnoreCase)
                ? data.OrderByDescending(x => x.Name)
                : data.OrderBy(x => x.Name);
            var items = ordered.ApplyPaging(p, ps);
            return Ok(new PagedResponse<TechnicianDto>(p, ps, data.Count, s, items));
        }

        [HttpGet("available")]
        [Authorize(Roles = Roles.Manager + "," + Roles.InstallationAdmin)]
        public async Task<IActionResult> GetAvailableTechnicians([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? sort)
        {
            if (!PaginationExtensions.TryNormalize(new PageRequest(page, pageSize, sort), out var p, out var ps, out var s, out var error))
            {
                return BadRequest(new { code = "invalid_pagination", message = error });
            }

            var result = await _mediator.Send(new GetAvailableTechniciansQuery());
            if (!result.Succeeded) return BadRequest(result);
            var data = result.Data ?? new List<TechnicianDto>();
            var ordered = s.Equals("name_desc", StringComparison.OrdinalIgnoreCase)
                ? data.OrderByDescending(x => x.Name)
                : data.OrderBy(x => x.Name);
            var items = ordered.ApplyPaging(p, ps);
            return Ok(new PagedResponse<TechnicianDto>(p, ps, data.Count, s, items));
        }

        [HttpPost("add")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> AddTechnician([FromBody] CreateTechnicianDto dto)
        {
            var result = await _mediator.Send(new AddTechnicianCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> UpdateTechnician(Guid id, [FromBody] UpdateTechnicianDto dto)
        {
            var result = await _mediator.Send(new UpdateTechnicianCommand(id, dto));
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("disable/{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> DisableTechnician(Guid id, [FromBody] bool disable)
        {
            var result = await _mediator.Send(new DisableTechnicianCommand(id, disable));
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> DeleteTechnician(Guid id)
        {
            var result = await _mediator.Send(new DeleteTechnicianCommand(id));
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get maintenance checklist items (for technicians to use when completing visits)
        [HttpGet("checklist-items")]
        public async Task<IActionResult> GetChecklistItems([FromQuery] bool includeInactive = false)
        {
            var technicianId = await GetTechnicianIdFromUserAsync();
            if (!technicianId.HasValue)
            {
                return Unauthorized(new { Message = "User is not linked to a technician." });
            }

            var result = await _mediator.Send(new ListChecklistItemsQuery(includeInactive));
            return Ok(result);
        }

        // Get technician's assigned emergency tickets
        [HttpGet("emergency-tickets")]
        public async Task<IActionResult> GetMyEmergencyTickets()
        {
            var technicianId = await GetTechnicianIdFromUserAsync();
            if (!technicianId.HasValue)
            {
                return Unauthorized(new { Message = "User is not linked to a technician." });
            }

            var result = await _mediator.Send(new GetEmergencyTicketsByTechnicianQuery 
            { 
                TechnicianId = technicianId.Value 
            });
            
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        private async Task<Guid?> GetTechnicianIdFromUserAsync()
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue) return null;

            var technician = await _technicianService.GetTechnicianByUserIdAsync(userId.Value);
            return technician?.Id;
        }
    }
}
