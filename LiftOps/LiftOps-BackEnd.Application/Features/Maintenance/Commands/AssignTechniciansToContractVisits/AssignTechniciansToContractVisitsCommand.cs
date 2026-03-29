using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.AssignTechniciansToContractVisits
{
    public class AssignTechniciansToContractVisitsCommand : IRequest<Result<Unit>>
    {
        public AssignTechniciansToContractVisitsDto Dto { get; set; } = null!;
    }

    public class AssignTechniciansToContractVisitsCommandHandler : IRequestHandler<AssignTechniciansToContractVisitsCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public AssignTechniciansToContractVisitsCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(AssignTechniciansToContractVisitsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var assignmentDate = request.Dto.AssignmentDate.Date;
                var month = assignmentDate.Month;
                var year = assignmentDate.Year;

                // Get all visits for this contract and month
                var visits = await _service.GetVisitsByContractAndMonthAsync(request.Dto.ContractId, month, year);
                
                // Filter visits for the specific date
                var visitsForDate = visits.Where(v => v.VisitDate.Date == assignmentDate).ToList();

                // If no visits exist for this date, create them for all elevators in the contract
                if (visitsForDate.Count == 0)
                {
                    // Get all elevators for this contract
                    var contract = await _service.GetContractByIdAsync(request.Dto.ContractId);
                    if (contract == null)
                    {
                        return Result<Unit>.Failure("Contract not found.");
                    }

                    var elevators = await _service.GetElevatorsByContractAsync(request.Dto.ContractId);
                    if (elevators == null || elevators.Count == 0)
                    {
                        return Result<Unit>.Failure("No elevators found for this contract.");
                    }

                    // Create visits for each elevator on the specified date
                    foreach (var elevator in elevators)
                    {
                        // Check if visit already exists (might exist but not in the filtered list)
                        var existingVisits = await _service.GetVisitsByElevatorAndMonthAsync(elevator.Id, month, year);
                        var existingVisitForDate = existingVisits.FirstOrDefault(v => v.VisitDate.Date == assignmentDate);
                        
                        if (existingVisitForDate == null)
                        {
                            // Schedule a new visit for this elevator
                            await _service.ScheduleVisitAsync(elevator.Id, assignmentDate, null);
                        }
                    }

                    // Refresh visits list after creating them
                    visits = await _service.GetVisitsByContractAndMonthAsync(request.Dto.ContractId, month, year);
                    visitsForDate = visits.Where(v => v.VisitDate.Date == assignmentDate).ToList();
                }

                if (visitsForDate.Count == 0)
                {
                    return Result<Unit>.Failure("No visits found or created for this contract on the specified date.");
                }

                // Distribute technicians to visits (round-robin if more technicians than visits)
                var technicianIndex = 0;
                foreach (var visit in visitsForDate)
                {
                    // Skip if visit is already done - completed visits cannot be reassigned
                    if (visit.Status == Domain.Entities.Maintenance.VisitStatus.Done)
                    {
                        continue;
                    }

                    // Assign technician (round-robin if multiple technicians)
                    var technicianId = request.Dto.TechnicianIds[technicianIndex % request.Dto.TechnicianIds.Count];
                    
                    // Check if this technician already completed maintenance for this elevator this month
                    // This prevents assigning the same technician to the same elevator twice in one month
                    var existingVisits = await _service.GetVisitsByElevatorAndMonthAsync(visit.MaintenanceElevatorId, month, year);
                    var hasCompletedVisit = existingVisits.Any(v => 
                        v.TechnicianId == technicianId && 
                        v.Status == Domain.Entities.Maintenance.VisitStatus.Done &&
                        v.VisitDate.Month == month &&
                        v.VisitDate.Year == year);

                    // Allow reassignment if:
                    // 1. Technician hasn't completed maintenance for this elevator this month, OR
                    // 2. Visit is from a previous day and wasn't completed (can be reassigned)
                    if (!hasCompletedVisit)
                    {
                        // Reassign technician - this will overwrite any previous assignment for incomplete visits
                        await _service.AssignTechnicianToVisitAsync(visit.Id, technicianId, request.Dto.Notes);
                    }

                    technicianIndex++;
                }

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
