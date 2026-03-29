using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Queries.GetMaintenanceContractDetails
{
    public class GetMaintenanceContractDetailsQuery : IRequest<MaintenanceContractDetailsDto?>
    {
        public Guid ContractId { get; set; }
    }

    public class GetMaintenanceContractDetailsQueryHandler : IRequestHandler<GetMaintenanceContractDetailsQuery, MaintenanceContractDetailsDto?>
    {
        private readonly IMaintenanceService _maintenanceService;

        public GetMaintenanceContractDetailsQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<MaintenanceContractDetailsDto?> Handle(GetMaintenanceContractDetailsQuery request, CancellationToken cancellationToken)
        {
            var contract = await _maintenanceService.GetContractByIdAsync(request.ContractId);
            if (contract == null) return null;

            return new MaintenanceContractDetailsDto
            {
                Id = contract.Id,
                CustomerId = contract.CustomerId,
                CustomerName = contract.Customer?.Name ?? string.Empty,
                CustomerPhone = contract.Customer?.Phone ?? string.Empty,
                CustomerEmail = contract.Customer?.Email ?? string.Empty,
                CustomerAddress = contract.Customer?.Address ?? string.Empty,
                CustomerCity = contract.Customer?.City ?? "القاهرة الجديدة",
                ProjectNumber = contract.ProjectNumber,
                ProjectAddress = !string.IsNullOrWhiteSpace(contract.ProjectAddress) ? contract.ProjectAddress : contract.Customer?.Address, // Use project address or fallback to customer address
                // Always use contract city if it exists (even if it's the default value "القاهرة الجديدة")
                // Only fallback to customer city if contract city is null or empty
                City = !string.IsNullOrEmpty(contract.City) ? contract.City : (contract.Customer?.City ?? "القاهرة الجديدة"),
                GoogleMapsLink = contract.GoogleMapsLink,
                IsFromInstallation = contract.IsFromInstallation,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                PricePerMonth = contract.PricePerMonth,
                FreeMonths = contract.FreeMonths,
                Status = contract.Status.ToString(),
                FrozenReason = contract.FrozenReason,
                FreezeEndDate = contract.FreezeEndDate,
                TechnicianId = contract.TechnicianId,
                TechnicianName = contract.Technician?.Name,
                CreatedAt = contract.CreatedAt,
                LastModifiedAt = contract.LastModifiedAt,
                Elevators = contract.Elevators?.Select(e => new MaintenanceElevatorDetailsDto
                {
                    Id = e.Id,
                    Type = e.Type,
                    NumberOfStops = e.NumberOfStops,
                    NumberOfFloors = e.NumberOfFloors,
                    NextMaintenanceDate = e.NextMaintenanceDate,
                    Status = e.Status.ToString(),
                    InstallationElevatorId = e.InstallationElevatorId,
                    CreatedAt = e.CreatedAt
                }).ToList() ?? new System.Collections.Generic.List<MaintenanceElevatorDetailsDto>()
            };
        }
    }
}

