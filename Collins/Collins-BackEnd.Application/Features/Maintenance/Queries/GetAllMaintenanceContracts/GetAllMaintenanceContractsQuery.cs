using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Collins_BackEnd.Application.Features.Maintenance.Queries.GetAllMaintenanceContracts
{
    public class GetAllMaintenanceContractsQuery : IRequest<List<MaintenanceContractListDto>>
    {
    }

    public class GetAllMaintenanceContractsQueryHandler : IRequestHandler<GetAllMaintenanceContractsQuery, List<MaintenanceContractListDto>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public GetAllMaintenanceContractsQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<List<MaintenanceContractListDto>> Handle(GetAllMaintenanceContractsQuery request, CancellationToken cancellationToken)
        {
            var contracts = await _maintenanceService.GetAllContractsAsync();

            return contracts.Select(c => new MaintenanceContractListDto
            {
                Id = c.Id,
                CustomerId = c.CustomerId,
                CustomerName = c.Customer?.Name ?? string.Empty,
                CustomerPhone = c.Customer?.Phone ?? string.Empty,
                CustomerEmail = c.Customer?.Email ?? string.Empty,
                CustomerAddress = c.Customer?.Address ?? string.Empty,
                CustomerCity = c.Customer?.City ?? "القاهرة الجديدة",
                ProjectNumber = c.ProjectNumber,
                ProjectAddress = !string.IsNullOrWhiteSpace(c.ProjectAddress) ? c.ProjectAddress : c.Customer?.Address, // Use project address or fallback to customer address
                City = !string.IsNullOrWhiteSpace(c.City) && c.City != "القاهرة الجديدة" ? c.City : (c.Customer?.City ?? "القاهرة الجديدة"), // Use project city or fallback to customer city
                GoogleMapsLink = c.GoogleMapsLink,
                IsFromInstallation = c.IsFromInstallation,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                PricePerMonth = c.PricePerMonth,
                FreeMonths = c.FreeMonths,
                Status = c.Status.ToString(),
                ElevatorCount = c.Elevators?.Count ?? 0,
                TechnicianId = c.TechnicianId,
                TechnicianName = c.Technician?.Name,
                CreatedAt = c.CreatedAt
            }).ToList();
        }
    }
}

