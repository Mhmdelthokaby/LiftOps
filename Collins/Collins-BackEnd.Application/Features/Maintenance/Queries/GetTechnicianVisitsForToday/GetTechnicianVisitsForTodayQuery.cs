using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Queries.GetTechnicianVisitsForToday
{
    public class GetTechnicianVisitsForTodayQuery : IRequest<Result<List<TechnicianVisitDto>>>
    {
        public Guid TechnicianId { get; set; }
        public DateTime? Date { get; set; } // Optional, defaults to today
    }

    public class GetTechnicianVisitsForTodayQueryHandler : IRequestHandler<GetTechnicianVisitsForTodayQuery, Result<List<TechnicianVisitDto>>>
    {
        private readonly IMaintenanceService _service;

        public GetTechnicianVisitsForTodayQueryHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<List<TechnicianVisitDto>>> Handle(GetTechnicianVisitsForTodayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var targetDate = request.Date?.Date ?? DateTime.UtcNow.Date;
                var month = targetDate.Month;
                var year = targetDate.Year;

                // Get all visits for the month/year
                var allVisits = await _service.GetMonthlyScheduleAsync(month, year);
                
                // Filter visits assigned to this technician for the specific date
                // Order by DisplayOrder to maintain the saved order per day
                var technicianVisits = allVisits
                    .Where(v => v.TechnicianId == request.TechnicianId && 
                                v.VisitDate.Date == targetDate)
                    .OrderBy(v => v.DisplayOrder)
                    .ThenBy(v => v.VisitDate)
                    .ToList();

                // Map to DTOs
                var visitDtos = technicianVisits.Select(v => new TechnicianVisitDto
                {
                    VisitId = v.Id,
                    ElevatorId = v.MaintenanceElevatorId,
                    ElevatorCode = v.MaintenanceElevator != null 
                        ? $"{v.MaintenanceElevator.Contract?.ProjectNumber ?? "N/A"}-M{v.MaintenanceElevator.Id.ToString().Substring(0, 8)}"
                        : "N/A",
                    ContractId = v.MaintenanceElevator?.ContractId ?? Guid.Empty,
                    ProjectNumber = v.MaintenanceElevator?.Contract?.ProjectNumber ?? "",
                    ProjectName = v.MaintenanceElevator?.Contract?.ProjectNumber ?? "",
                    ProjectNotes = null, // MaintenanceContract doesn't have Notes property
                    CustomerId = v.MaintenanceElevator?.Contract?.CustomerId ?? Guid.Empty,
                    CustomerName = v.MaintenanceElevator?.Contract?.Customer?.Name ?? "",
                    CustomerPhone = v.MaintenanceElevator?.Contract?.Customer?.Phone ?? "",
                    CustomerAddress = v.MaintenanceElevator?.Contract?.ProjectAddress ?? 
                                     v.MaintenanceElevator?.Contract?.Customer?.Address ?? "",
                    City = v.MaintenanceElevator?.Contract?.City ?? 
                           v.MaintenanceElevator?.Contract?.Customer?.City ?? "",
                    GoogleMapsLink = v.MaintenanceElevator?.Contract?.GoogleMapsLink,
                    VisitDate = v.VisitDate,
                    Status = v.Status.ToString(),
                    Notes = v.Notes,
                    PaymentNotes = v.PaymentNotes,
                    IsPaid = v.IsPaid,
                    CompletedDate = v.CompletedDate
                }).ToList();

                return Result<List<TechnicianVisitDto>>.Success(visitDtos);
            }
            catch (Exception ex)
            {
                return Result<List<TechnicianVisitDto>>.Failure(ex.Message);
            }
        }
    }
}
