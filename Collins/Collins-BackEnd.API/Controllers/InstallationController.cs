using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Features.Installation.Commands;
using Collins_BackEnd.Application.Features.Installation.Queries;
using Collins_BackEnd.Application.Features.Installation.Queries; // General Queries namespace
using Collins_BackEnd.Application.Features.Technicians.Commands;
using Collins_BackEnd.Application.Common;
using Collins_BackEnd.Domain.Common;
using Collins_BackEnd.Domain.Entities.Installation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Collins_BackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireInstallation")] // Manager or InstallationAdmin
    public class InstallationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InstallationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("project/add")]
        public async Task<ActionResult<Result<Guid>>> CreateProject([FromBody] CreateProjectDto dto)
        {
            // Get InstallationAdminId from claims (or use a default for now)
            var installationAdminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var installationAdminId = installationAdminIdClaim != null && Guid.TryParse(installationAdminIdClaim, out var adminId) 
                ? adminId 
                : Guid.NewGuid(); // Fallback for development/testing

            var command = new CreateInstallationProjectCommand
            {
                ProjectDto = dto,
                InstallationAdminId = installationAdminId
            };

            return await _mediator.Send(command);
        }

        [HttpPost("project/{projectId}/elevator/add")]
        public async Task<ActionResult<Result<Guid>>> AddElevator(Guid projectId, [FromBody] CreateElevatorDto elevatorDto)
        {
            var command = new AddElevatorToProjectCommand 
            { 
                ElevatorDto = new AddElevatorDto { ProjectId = projectId, Elevator = elevatorDto } 
            };
            return await _mediator.Send(command);
        }

        [HttpPut("elevator/{elevatorId}")]
        public async Task<ActionResult<Result<Unit>>> UpdateElevator(Guid elevatorId, [FromBody] CreateElevatorDto elevatorDto)
        {
            var command = new UpdateElevatorCommand
            {
                ElevatorId = elevatorId,
                ElevatorDto = elevatorDto
            };
            return await _mediator.Send(command);
        }

        [HttpPost("stage/start")]
        public async Task<ActionResult<Result<Unit>>> StartStage([FromBody] StartStageDto dto)
        {
            return await _mediator.Send(new StartStageCommand { Dto = dto });
        }

        [HttpPost("stage/complete")]
        public async Task<ActionResult<Result<Unit>>> CompleteStage([FromBody] CompleteStageDto dto)
        {
            return await _mediator.Send(new CompleteStageCommand { Dto = dto });
        }

        [HttpPost("stage/parts")]
        public async Task<ActionResult<Result<Unit>>> AddStageParts([FromBody] AddStagePartsDto dto)
        {
            return await _mediator.Send(new AddStagePartsCommand { Dto = dto });
        }

        [HttpPut("stage/update")]
        public async Task<ActionResult<Result<Unit>>> UpdateStage([FromBody] UpdateStageDto dto)
        {
            return await _mediator.Send(new UpdateStageCommand { Dto = dto });
        }

        [HttpGet("projects")]
        public async Task<ActionResult<Result<IReadOnlyList<InstallationProjectDto>>>> GetProjects([FromQuery] ProjectStatus? status)
        {
            return await _mediator.Send(new GetInstallationProjectsQuery { StatusFilter = status });
        }

        [HttpGet("project/{id}")]
        public async Task<ActionResult<Result<InstallationProjectDto>>> GetProjectDetails(Guid id)
        {
            return await _mediator.Send(new GetProjectDetailsQuery { ProjectId = id });
        }

        [HttpPost("project/{id}/approve-inspection")]
        public async Task<ActionResult<Result<Unit>>> ApproveInspection(Guid id)
        {
            return await _mediator.Send(new ApproveInspectionCommand { ProjectId = id });
        }

        [HttpPost("project/{id}/reject")]
        public async Task<ActionResult<Result<Unit>>> RejectProject(Guid id)
        {
            return await _mediator.Send(new RejectProjectCommand { ProjectId = id });
        }

        [HttpPut("project/{id}")]
        public async Task<ActionResult<Result<Unit>>> UpdateProject(Guid id, [FromBody] UpdateProjectDto dto)
        {
            dto.ProjectId = id;
            return await _mediator.Send(new UpdateInstallationProjectCommand { ProjectDto = dto });
        }

        [HttpGet("stage/{id}")]
        public async Task<ActionResult<Result<InstallationStageDto>>> GetStageDetails(Guid id)
        {
            return await _mediator.Send(new GetStageDetailsQuery { StageId = id });
        }

        [HttpPost("projects/fix-numbers")]
        public async Task<ActionResult<Result<int>>> FixProjectNumbers()
        {
            return await _mediator.Send(new FixProjectNumbersCommand());
        }

        [HttpGet("check-project-number")]
        public async Task<ActionResult<Result<bool>>> CheckProjectNumber([FromQuery] string projectNumber)
        {
            if (string.IsNullOrWhiteSpace(projectNumber))
            {
                return Ok(Result<bool>.Success(false));
            }

            var exists = await _mediator.Send(new CheckProjectNumberExistsQuery { ProjectNumber = projectNumber });
            return Ok(Result<bool>.Success(exists));
        }

        [HttpGet("notifications")]
        public async Task<ActionResult<Result<IReadOnlyList<NotificationDto>>>> GetNotifications()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out var userId);
            
            return await _mediator.Send(new GetNotificationsQuery { UserId = userId });
        }

        // Technician Assignment Endpoints
        
        [HttpPost("elevator/{elevatorId}/assign-technicians")]
        [Authorize(Roles = Roles.Manager + "," + Roles.InstallationAdmin)]
        public async Task<IActionResult> AssignTechnicians(Guid elevatorId, [FromBody] AssignTechnicianDto dto)
        {
            dto.ElevatorId = elevatorId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var command = new AssignTechnicianCommand 
            { 
                Dto = dto,
                AssignedByUserId = userId != null ? Guid.Parse(userId) : Guid.Empty 
            };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(new { Message = "Technicians assigned successfully." });
        }

        [HttpDelete("elevator/{elevatorId}/unassign-technician/{techId}")]
        [Authorize(Roles = Roles.Manager + "," + Roles.InstallationAdmin)]
        public async Task<IActionResult> UnassignTechnician(Guid elevatorId, Guid techId)
        {
            var result = await _mediator.Send(new UnassignTechnicianCommand(techId, elevatorId));
            if (!result.Succeeded) return BadRequest(result);
            return Ok(new { Message = "Technician unassigned successfully." });
        }

        // ========== Inspection & Offer Workflow Endpoints ==========

        [HttpPost("inspection/create")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Guid>>> CreateInspectionRequest([FromBody] CreateInspectionRequestDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out var adminId);

            var command = new CreateInspectionRequestCommand
            {
                Dto = dto,
                InstallationAdminId = adminId
            };
            return await _mediator.Send(command);
        }

        [HttpPut("inspection/{inspectionId}/technical-data")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<bool>>> UpdateInspectionTechnicalData(
            Guid inspectionId,
            [FromBody] UpdateInspectionTechnicalDataDto dto)
        {
            var command = new UpdateInspectionTechnicalDataCommand
            {
                InspectionId = inspectionId,
                Dto = dto
            };
            return await _mediator.Send(command);
        }

        [HttpGet("inspections")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<List<InspectionRequestDto>>>> GetInspectionRequests(
            [FromQuery] InspectionStatus? status)
        {
            var query = new GetInspectionRequestsQuery
            {
                StatusFilter = status
            };
            return await _mediator.Send(query);
        }

        [HttpGet("inspection/{id}")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<InspectionRequestDto>>> GetInspectionRequestDetails(Guid id)
        {
            var query = new GetInspectionRequestDetailsQuery { InspectionId = id };
            return await _mediator.Send(query);
        }

        [HttpPost("offer/create")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Guid>>> CreateOffer([FromBody] CreateOfferDto dto)
        {
            var command = new CreateOfferCommand { Dto = dto };
            return await _mediator.Send(command);
        }

        [HttpPut("offer/{offerId}")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<bool>>> UpdateOffer(
            Guid offerId,
            [FromBody] UpdateOfferDto dto)
        {
            var command = new UpdateOfferCommand
            {
                OfferId = offerId,
                Dto = dto
            };
            return await _mediator.Send(command);
        }

        [HttpPut("offer/{offerId}/pdf")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<bool>>> UpdateOfferPdf(
            Guid offerId,
            [FromBody] UpdateOfferPdfDto dto)
        {
            var command = new UpdateOfferPdfCommand
            {
                OfferId = offerId,
                OfferPdfPath = dto.OfferPdfPath
            };
            return await _mediator.Send(command);
        }

        [HttpPut("offer/{offerId}/approve")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<bool>>> ApproveOffer(
            Guid offerId,
            [FromBody] ApproveOfferDto dto)
        {
            dto.OfferId = offerId;
            var isManager = User.IsInRole(Roles.Manager);
            
            var command = new ApproveOfferCommand
            {
                Dto = dto,
                IsManagerOverride = isManager
            };
            return await _mediator.Send(command);
        }

        [HttpGet("offers")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<List<OfferDto>>>> GetOffers([FromQuery] OfferStatus? status)
        {
            var query = new GetOffersQuery { StatusFilter = status };
            return await _mediator.Send(query);
        }

        [HttpPost("offer/{offerId}/convert-to-project")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Guid>>> ConvertOfferToProject(Guid offerId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out var adminId);

            var command = new ConvertOfferToProjectCommand
            {
                OfferId = offerId,
                InstallationAdminId = adminId
            };
            return await _mediator.Send(command);
        }

        // ========== New Inspection Project Flow Endpoints ==========

        [HttpPost("inspection-project/create")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Guid>>> CreateInspectionProject([FromBody] CreateInspectionProjectDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out var adminId);

            var command = new CreateInspectionProjectCommand
            {
                Dto = dto,
                InstallationAdminId = adminId
            };
            return await _mediator.Send(command);
        }

        [HttpGet("inspection-projects")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<List<InspectionProjectDto>>>> GetInspectionProjects(
            [FromQuery] ProjectStatus? status)
        {
            var query = new GetInspectionProjectsQuery
            {
                StatusFilter = status
            };
            return await _mediator.Send(query);
        }

        [HttpPost("quotation/create")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Guid>>> CreateQuotation([FromBody] CreateQuotationDto dto)
        {
            var command = new CreateQuotationCommand { Dto = dto };
            return await _mediator.Send(command);
        }

        [HttpPost("quotation/{quotationId}/approve")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Unit>>> ApproveQuotation(
            Guid quotationId,
            [FromBody] ApproveRejectQuotationDto? dto)
        {
            if (dto == null)
            {
                dto = new ApproveRejectQuotationDto { QuotationId = quotationId };
            }
            else
            {
                dto.QuotationId = quotationId;
            }

            var command = new ApproveQuotationCommand { Dto = dto };
            return await _mediator.Send(command);
        }

        [HttpPost("quotation/{quotationId}/reject")]
        [Authorize(Policy = "RequireInstallation")]
        public async Task<ActionResult<Result<Unit>>> RejectQuotation(
            Guid quotationId,
            [FromBody] ApproveRejectQuotationDto? dto)
        {
            if (dto == null)
            {
                dto = new ApproveRejectQuotationDto { QuotationId = quotationId };
            }
            else
            {
                dto.QuotationId = quotationId;
            }

            var command = new RejectQuotationCommand { Dto = dto };
            return await _mediator.Send(command);
        }
    }
}
