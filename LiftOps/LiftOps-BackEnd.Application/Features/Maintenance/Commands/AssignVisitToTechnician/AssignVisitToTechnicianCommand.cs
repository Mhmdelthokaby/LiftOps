using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.AssignVisitToTechnician
{
    public class AssignVisitToTechnicianCommand : IRequest<Result<Unit>>
    {
        public AssignVisitToTechnicianDto Dto { get; set; } = null!;
    }

    public class AssignVisitToTechnicianCommandHandler : IRequestHandler<AssignVisitToTechnicianCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public AssignVisitToTechnicianCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(AssignVisitToTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if visit already has a technician assigned for this month
                var visit = await _service.GetVisitByIdAsync(request.Dto.VisitId);
                if (visit == null)
                {
                    return Result<Unit>.Failure("Visit not found.");
                }

                if (request.Dto.TechnicianId.HasValue)
                {
                    // Check if technician already has a visit for this elevator in the same month
                    var visitDate = request.Dto.AssignmentDate.Date;
                    var month = visitDate.Month;
                    var year = visitDate.Year;
                    
                    var existingVisits = await _service.GetVisitsByElevatorAndMonthAsync(visit.MaintenanceElevatorId, month, year);
                    var hasCompletedVisit = existingVisits.Any(v => 
                        v.TechnicianId == request.Dto.TechnicianId.Value && 
                        v.Status == Domain.Entities.Maintenance.VisitStatus.Done &&
                        v.VisitDate.Month == month &&
                        v.VisitDate.Year == year);

                    if (hasCompletedVisit)
                    {
                        return Result<Unit>.Failure($"Technician already completed maintenance for this elevator in {visitDate:MMMM yyyy}. Only one maintenance visit is allowed per month.");
                    }
                }

                // Assign technician to visit with notes
                await _service.AssignTechnicianToVisitAsync(request.Dto.VisitId, request.Dto.TechnicianId, request.Dto.Notes);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
