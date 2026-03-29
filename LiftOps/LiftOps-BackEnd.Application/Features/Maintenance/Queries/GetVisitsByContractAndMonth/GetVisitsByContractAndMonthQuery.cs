using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Queries.GetVisitsByContractAndMonth
{
    public class GetVisitsByContractAndMonthQuery : IRequest<Result<List<MaintenanceVisitListDto>>>
    {
        public Guid ContractId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class GetVisitsByContractAndMonthQueryHandler : IRequestHandler<GetVisitsByContractAndMonthQuery, Result<List<MaintenanceVisitListDto>>>
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMapper _mapper;

        public GetVisitsByContractAndMonthQueryHandler(IMaintenanceService maintenanceService, IMapper mapper)
        {
            _maintenanceService = maintenanceService;
            _mapper = mapper;
        }

        public async Task<Result<List<MaintenanceVisitListDto>>> Handle(GetVisitsByContractAndMonthQuery request, CancellationToken cancellationToken)
        {
            if (request.Month < 1 || request.Month > 12)
            {
                return Result<List<MaintenanceVisitListDto>>.Failure("Month must be between 1 and 12.");
            }

            if (request.Year < 2000 || request.Year > 2100)
            {
                return Result<List<MaintenanceVisitListDto>>.Failure("Year must be between 2000 and 2100.");
            }

            var visits = await _maintenanceService.GetVisitsByContractAndMonthAsync(request.ContractId, request.Month, request.Year);
            
            var visitDtos = visits.Select(v => new MaintenanceVisitListDto
            {
                Id = v.Id,
                VisitDate = v.VisitDate,
                Status = v.Status.ToString(),
                Notes = v.Notes,
                PaymentNotes = v.PaymentNotes,
                IsPaid = v.IsPaid,
                CompletedDate = v.CompletedDate,
                ElevatorId = v.MaintenanceElevatorId,
                ElevatorCode = v.MaintenanceElevator != null 
                    ? $"{v.MaintenanceElevator.Contract?.ProjectNumber ?? "N/A"}-M{v.MaintenanceElevator.Id.ToString().Substring(0, 8)}"
                    : "N/A",
                TechnicianId = v.TechnicianId,
                ChecklistItems = v.ChecklistItems?.Select(ci => new MaintenanceVisitChecklistItemDto
                {
                    ChecklistItemId = ci.ChecklistItemId,
                    ChecklistItemTitle = ci.ChecklistItem?.Title ?? "",
                    IsCompleted = ci.IsCompleted,
                    Notes = ci.Notes,
                    Count = ci.Count,
                    Percentage = ci.Percentage
                }).ToList() ?? new List<MaintenanceVisitChecklistItemDto>()
            }).ToList();

            return Result<List<MaintenanceVisitListDto>>.Success(visitDtos);
        }
    }
}

