using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Application.Features.Maintenance.Commands;
using Collins_BackEnd.Application.Features.Maintenance.Commands.AddChecklistItem;
using Collins_BackEnd.Application.Features.Maintenance.Commands.UpdateChecklistItem;
using Collins_BackEnd.Application.Features.Maintenance.Commands.DeleteChecklistItem;
using Collins_BackEnd.Application.Features.Maintenance.Commands.CreateMaintenanceProject;
using Collins_BackEnd.Application.Features.Maintenance.Commands.MarkVisitAsPaid;
using Collins_BackEnd.Application.Features.Maintenance.Commands.FreezeElevator;
using Collins_BackEnd.Application.Features.Maintenance.Commands.StopElevator;
using Collins_BackEnd.Application.Features.Maintenance.Commands.ActivateElevator;
using Collins_BackEnd.Application.Features.Maintenance.Commands.FreezeContract;
using Collins_BackEnd.Application.Features.Maintenance.Commands.StopContract;
using Collins_BackEnd.Application.Features.Maintenance.Commands.ActivateContract;
using Collins_BackEnd.Application.Features.Maintenance.Queries.ListChecklistItems;
using Collins_BackEnd.Application.Features.Maintenance.Queries.GetAllMaintenanceContracts;
using Collins_BackEnd.Application.Features.Maintenance.Queries.CheckProjectNumberExists;
using Collins_BackEnd.Application.Features.Maintenance.Queries.GetMaintenanceContractDetails;
using Collins_BackEnd.Application.Features.Maintenance.Queries.GetAllMaintenanceElevators;
using Collins_BackEnd.Application.Features.Maintenance.Queries.GetVisitsByContractAndMonth;
using Collins_BackEnd.Application.Features.Maintenance.Queries.GetVisitsByElevator;
using Collins_BackEnd.Application.Features.Maintenance.Queries.GetMaintenanceStatistics;
using Collins_BackEnd.Application.Features.Maintenance.Commands.AssignVisitToTechnician;
using Collins_BackEnd.Application.Features.Maintenance.Commands.AssignTechniciansToContractVisits;
using Collins_BackEnd.Application.Features.Maintenance.Commands.UpdateVisitStatus;
using Collins_BackEnd.Application.Features.Maintenance.Commands.UpdateVisitOrder;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Collins_BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin)]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPdfGenerator _pdfGenerator;

        public MaintenanceController(IMediator mediator, IPdfGenerator pdfGenerator)
        {
            _mediator = mediator;
            _pdfGenerator = pdfGenerator;
        }

        [HttpPost("add-contract")]
        public async Task<IActionResult> AddContract([FromBody] CreateMaintenanceContractDto dto)
        {
            var result = await _mediator.Send(new CreateMaintenanceContractCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("visit/schedule")]
        public async Task<IActionResult> ScheduleVisit([FromBody] ScheduleVisitDto dto)
        {
            var result = await _mediator.Send(new ScheduleVisitCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
        
        [HttpPost("visit/{visitId}/complete")]
        [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin + "," + Roles.Technician)]
        public async Task<IActionResult> CompleteVisit(Guid visitId, [FromBody] CompleteVisitDto dto)
        {
            dto.VisitId = visitId;
            var result = await _mediator.Send(new CompleteVisitCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("visit/{visitId}")]
        public async Task<IActionResult> GetVisitDetails(Guid visitId)
        {
            var service = HttpContext.RequestServices.GetRequiredService<Collins_BackEnd.Application.Interfaces.Maintenance.IMaintenanceService>();
            var visit = await service.GetVisitByIdAsync(visitId);
            if (visit == null) return NotFound(new { Message = "Visit not found." });
            
            // Map checklist items
            var checklistItems = visit.ChecklistItems != null
                ? visit.ChecklistItems.Select(ci => new
                {
                    ChecklistItemId = ci.ChecklistItemId,
                    ChecklistItemTitle = ci.ChecklistItem?.Title ?? "",
                    IsCompleted = ci.IsCompleted,
                    Notes = ci.Notes,
                    Count = ci.Count,
                    Percentage = ci.Percentage
                }).Cast<object>().ToList()
                : new List<object>();
            
            // Map spare parts
            var spareParts = visit.SpareParts != null
                ? visit.SpareParts.Select(sp => new
                {
                    ItemId = sp.InventoryItemId,
                    ItemName = sp.InventoryItem?.Name ?? "",
                    Quantity = sp.Quantity,
                    PriceAtTimeOfUsage = sp.PriceAtTimeOfUsage,
                    TotalPrice = sp.Quantity * sp.PriceAtTimeOfUsage,
                    IsPaid = sp.IsPaid
                }).Cast<object>().ToList()
                : new List<object>();
            
            // Map to DTO
            var visitDto = new
            {
                Id = visit.Id,
                VisitDate = visit.VisitDate,
                Status = visit.Status.ToString(),
                Notes = visit.Notes,
                IsPaid = visit.IsPaid,
                CompletedDate = visit.CompletedDate,
                ElevatorId = visit.MaintenanceElevatorId,
                ElevatorCode = visit.MaintenanceElevator != null 
                    ? $"{visit.MaintenanceElevator.Contract?.ProjectNumber ?? "N/A"}-M{visit.MaintenanceElevator.Id.ToString().Substring(0, 8)}"
                    : "N/A",
                TechnicianId = visit.TechnicianId,
                TechnicianName = visit.Technician?.Name ?? null,
                ProjectNumber = visit.MaintenanceElevator?.Contract?.ProjectNumber ?? null,
                CustomerName = visit.MaintenanceElevator?.Contract?.Customer?.Name ?? null,
                CustomerAddress = visit.MaintenanceElevator?.Contract?.Customer?.Address ?? null,
                ChecklistItems = checklistItems,
                SpareParts = spareParts
            };
            
            return Ok(visitDto);
        }

        // Checklist Item Management Endpoints
        [HttpPost("checklist-item/add")]
        public async Task<IActionResult> AddChecklistItem([FromBody] CreateMaintenanceChecklistItemDto dto)
        {
            var result = await _mediator.Send(new AddChecklistItemCommand(dto));
            if (result == null) return BadRequest(new { Message = "Failed to add checklist item." });
            return Ok(new { Id = result, Message = "Checklist item added successfully." });
        }

        [HttpPut("checklist-item/{id}")]
        public async Task<IActionResult> UpdateChecklistItem(Guid id, [FromBody] UpdateMaintenanceChecklistItemDto dto)
        {
            var result = await _mediator.Send(new UpdateChecklistItemCommand(id, dto));
            if (!result) return BadRequest(new { Message = "Failed to update checklist item." });
            return Ok(new { Message = "Checklist item updated successfully." });
        }

        [HttpDelete("checklist-item/{id}")]
        public async Task<IActionResult> DeleteChecklistItem(Guid id)
        {
            var result = await _mediator.Send(new DeleteChecklistItemCommand(id));
            if (!result) return BadRequest(new { Message = "Failed to delete checklist item." });
            return Ok(new { Message = "Checklist item deleted successfully." });
        }

        [HttpGet("checklist-item/list")]
        [Authorize(Roles = Roles.Manager + "," + Roles.MaintenanceAdmin + "," + Roles.Technician)]
        public async Task<IActionResult> ListChecklistItems([FromQuery] bool includeInactive = false)
        {
            var result = await _mediator.Send(new ListChecklistItemsQuery(includeInactive));
            return Ok(result);
        }

        // Maintenance Projects Management
        [HttpGet("projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _mediator.Send(new GetAllMaintenanceContractsQuery());
            return Ok(result);
        }

        [HttpGet("projects/{contractId}")]
        public async Task<IActionResult> GetProjectDetails(Guid contractId)
        {
            var result = await _mediator.Send(new GetMaintenanceContractDetailsQuery { ContractId = contractId });
            if (result == null) return NotFound(new { Message = "Maintenance contract not found." });
            return Ok(result);
        }

        [HttpGet("check-project-number")]
        public async Task<IActionResult> CheckProjectNumber([FromQuery] string projectNumber)
        {
            if (string.IsNullOrWhiteSpace(projectNumber))
                return BadRequest(new { Message = "Project number is required." });
            
            var exists = await _mediator.Send(new CheckProjectNumberExistsQuery { ProjectNumber = projectNumber });
            return Ok(new { Exists = exists });
        }

        [HttpPost("projects/create")]
        public async Task<IActionResult> CreateProject([FromBody] CreateMaintenanceProjectDto dto)
        {
            var result = await _mediator.Send(new CreateMaintenanceProjectCommand { ProjectDto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("projects/{contractId}")]
        public async Task<IActionResult> UpdateProject(Guid contractId, [FromBody] UpdateMaintenanceContractDto dto)
        {
            var result = await _mediator.Send(new UpdateMaintenanceContractCommand 
            { 
                ContractId = contractId, 
                Dto = dto 
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("elevators/{elevatorId}")]
        public async Task<IActionResult> UpdateElevator(Guid elevatorId, [FromBody] UpdateMaintenanceElevatorDto dto)
        {
            var result = await _mediator.Send(new UpdateMaintenanceElevatorCommand 
            { 
                ElevatorId = elevatorId, 
                Dto = dto 
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("visit/{visitId}/mark-paid")]
        public async Task<IActionResult> MarkVisitAsPaid(Guid visitId)
        {
            var result = await _mediator.Send(new MarkVisitAsPaidCommand { VisitId = visitId });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("schedule/monthly")]
        public async Task<IActionResult> GetMonthlySchedule([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12) return BadRequest(new { Message = "Month must be between 1 and 12." });
            if (year < 2000 || year > 2100) return BadRequest(new { Message = "Year must be between 2000 and 2100." });
            
            var service = HttpContext.RequestServices.GetRequiredService<Collins_BackEnd.Application.Interfaces.Maintenance.IMaintenanceService>();
            var visits = await service.GetMonthlyScheduleAsync(month, year);
            
            // Map to DTO
            var visitDtos = visits.Select(v => new
            {
                Id = v.Id,
                VisitDate = v.VisitDate,
                Status = v.Status.ToString(),
                Notes = v.Notes,
                IsPaid = v.IsPaid,
                TechnicianId = v.TechnicianId,
                TechnicianName = v.Technician?.Name ?? "Unassigned",
                ElevatorId = v.MaintenanceElevatorId,
                ElevatorCode = v.MaintenanceElevator != null 
                    ? $"{v.MaintenanceElevator.Contract?.ProjectNumber ?? "N/A"}-M{v.MaintenanceElevator.Id.ToString().Substring(0, 8)}"
                    : "N/A",
                ElevatorType = v.MaintenanceElevator?.Type ?? "Unknown",
                ElevatorStops = v.MaintenanceElevator?.NumberOfStops ?? 0,
                ElevatorFloors = v.MaintenanceElevator?.NumberOfFloors ?? 0,
                ProjectNumber = v.MaintenanceElevator?.Contract?.ProjectNumber ?? "",
                CustomerName = v.MaintenanceElevator?.Contract?.Customer?.Name ?? "",
                CustomerAddress = v.MaintenanceElevator?.Contract?.Customer?.Address ?? ""
            }).ToList();
            
            return Ok(visitDtos);
        }

        [HttpGet("elevators")]
        public async Task<IActionResult> GetAllElevators()
        {
            var result = await _mediator.Send(new GetAllMaintenanceElevatorsQuery());
            return Ok(result);
        }

        // Elevator Status Management
        [HttpPost("elevator/{elevatorId}/freeze")]
        public async Task<IActionResult> FreezeElevator(Guid elevatorId, [FromBody] FreezeElevatorDto? dto = null)
        {
            var result = await _mediator.Send(new FreezeElevatorCommand 
            { 
                ElevatorId = elevatorId,
                Reason = dto?.Reason,
                FreezeEndDate = dto?.FreezeEndDate
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("elevator/{elevatorId}/stop")]
        public async Task<IActionResult> StopElevator(Guid elevatorId, [FromBody] StopElevatorDto? dto = null)
        {
            var result = await _mediator.Send(new StopElevatorCommand 
            { 
                ElevatorId = elevatorId,
                Reason = dto?.Reason
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("elevator/{elevatorId}/activate")]
        public async Task<IActionResult> ActivateElevator(Guid elevatorId)
        {
            var result = await _mediator.Send(new ActivateElevatorCommand { ElevatorId = elevatorId });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Contract Status Management
        [HttpPost("contract/{contractId}/freeze")]
        public async Task<IActionResult> FreezeContract(Guid contractId, [FromBody] FreezeContractDto? dto = null)
        {
            var result = await _mediator.Send(new FreezeContractCommand 
            { 
                ContractId = contractId,
                Reason = dto?.Reason,
                FreezeEndDate = dto?.FreezeEndDate
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("contract/{contractId}/stop")]
        public async Task<IActionResult> StopContract(Guid contractId, [FromBody] StopContractDto? dto = null)
        {
            var result = await _mediator.Send(new StopContractCommand 
            { 
                ContractId = contractId,
                Reason = dto?.Reason
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("contract/{contractId}/activate")]
        public async Task<IActionResult> ActivateContract(Guid contractId)
        {
            var result = await _mediator.Send(new ActivateContractCommand { ContractId = contractId });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("contract/{contractId}/visits")]
        public async Task<IActionResult> GetVisitsByContractAndMonth(Guid contractId, [FromQuery] int month, [FromQuery] int year)
        {
            var result = await _mediator.Send(new GetVisitsByContractAndMonthQuery 
            { 
                ContractId = contractId, 
                Month = month, 
                Year = year 
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("elevator/{elevatorId}/visits")]
        public async Task<IActionResult> GetVisitsByElevator(Guid elevatorId, [FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            var result = await _mediator.Send(new GetVisitsByElevatorQuery 
            { 
                ElevatorId = elevatorId, 
                Month = month, 
                Year = year 
            });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("visit/{visitId}/pdf")]
        [AllowAnonymous] // Allow public access (or keep it authorized if preferred)
        public async Task<IActionResult> DownloadPdf(Guid visitId)
        {
            var service = HttpContext.RequestServices.GetRequiredService<IMaintenanceService>();
            var visit = await service.GetVisitByIdAsync(visitId);
            
            if (visit == null) return NotFound(new { Message = "Visit not found." });

            var pdfBytes = await _pdfGenerator.GenerateMaintenanceVisitReportAsync(visit);
            
            var fileName = $"Maintenance_Report_{visit.MaintenanceElevator?.Contract?.ProjectNumber ?? visitId.ToString()}_{visit.VisitDate:yyyyMMdd}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }

        // Assign visit to technician for a specific day
        [HttpPost("visit/assign-technician")]
        public async Task<IActionResult> AssignVisitToTechnician([FromBody] AssignVisitToTechnicianDto dto)
        {
            var result = await _mediator.Send(new AssignVisitToTechnicianCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Update visit status (InProgress, Done)
        [HttpPut("visit/{visitId}/status")]
        public async Task<IActionResult> UpdateVisitStatus(Guid visitId, [FromBody] UpdateVisitStatusDto dto)
        {
            dto.VisitId = visitId;
            var result = await _mediator.Send(new UpdateVisitStatusCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Cancel all incomplete visits for a specific date
        [HttpPost("visits/cancel-incomplete-by-date")]
        public async Task<IActionResult> CancelIncompleteVisitsByDate([FromBody] CancelVisitsByDateDto dto)
        {
            var service = HttpContext.RequestServices.GetRequiredService<IMaintenanceService>();
            var count = await service.CancelIncompleteVisitsByDateAsync(dto.Date);
            return Ok(new { Message = $"Cancelled {count} incomplete visit(s).", Count = count });
        }

        // Update visit order for a specific date
        [HttpPost("visits/update-order")]
        public async Task<IActionResult> UpdateVisitOrder([FromBody] UpdateVisitOrderDto dto)
        {
            var result = await _mediator.Send(new UpdateVisitOrderCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(new { Message = "Visit order updated successfully." });
        }

        // Assign technicians to contract visits for a specific date
        [HttpPost("contract/assign-technicians")]
        public async Task<IActionResult> AssignTechniciansToContractVisits([FromBody] AssignTechniciansToContractVisitsDto dto)
        {
            var result = await _mediator.Send(new AssignTechniciansToContractVisitsCommand { Dto = dto });
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        // Get maintenance statistics for active contracts only
        [HttpGet("statistics")]
        public async Task<IActionResult> GetMaintenanceStatistics([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12) return BadRequest(new { Message = "Month must be between 1 and 12." });
            if (year < 2000 || year > 2100) return BadRequest(new { Message = "Year must be between 2000 and 2100." });

            var result = await _mediator.Send(new GetMaintenanceStatisticsQuery 
            { 
                Month = month, 
                Year = year 
            });
            return Ok(result);
        }
    }
}
