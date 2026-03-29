using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Queries.GetAllMaintenanceElevators
{
    public class GetAllMaintenanceElevatorsQuery : IRequest<List<MaintenanceElevatorListDto>>
    {
    }

    public class GetAllMaintenanceElevatorsQueryHandler : IRequestHandler<GetAllMaintenanceElevatorsQuery, List<MaintenanceElevatorListDto>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public GetAllMaintenanceElevatorsQueryHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<List<MaintenanceElevatorListDto>> Handle(GetAllMaintenanceElevatorsQuery request, CancellationToken cancellationToken)
        {
            var elevators = await _maintenanceService.GetAllElevatorsAsync();

            // Group elevators by contract to generate codes (M1, M2, etc.)
            var elevatorsByContract = elevators
                .GroupBy(e => e.ContractId)
                .ToList();

            var result = new List<MaintenanceElevatorListDto>();
            
            foreach (var contractGroup in elevatorsByContract)
            {
                int elevatorIndex = 1;
                var firstElevator = contractGroup.First();
                var contract = firstElevator.Contract;
                
                // Ensure contract has a project number
                if (contract != null && string.IsNullOrWhiteSpace(contract.ProjectNumber))
                {
                    await _maintenanceService.EnsureContractHasProjectNumberAsync(contract);
                }
                
                foreach (var elevator in contractGroup.OrderBy(e => e.CreatedAt))
                {
                    // Use the contract we already have (all elevators in group share the same contract)
                    var customer = contract?.Customer;
                    
                    // Generate elevator code: ProjectNumber-M1, M2, etc.
                    string elevatorCode = string.Empty;
                    if (!string.IsNullOrWhiteSpace(contract?.ProjectNumber))
                    {
                        elevatorCode = $"{contract.ProjectNumber}-M{elevatorIndex}";
                    }
                    else
                    {
                        elevatorCode = $"M{elevatorIndex}";
                    }

                    result.Add(new MaintenanceElevatorListDto
                    {
                        Id = elevator.Id,
                        ElevatorCode = elevatorCode,
                        ContractId = elevator.ContractId,
                        ProjectNumber = contract?.ProjectNumber ?? string.Empty,
                        CustomerId = customer?.Id ?? Guid.Empty,
                        CustomerName = customer?.Name ?? string.Empty,
                        CustomerPhone = customer?.Phone ?? string.Empty,
                        CustomerEmail = customer?.Email ?? string.Empty,
                        CustomerAddress = customer?.Address ?? string.Empty,
                        CustomerCity = customer?.City ?? "القاهرة الجديدة",
                        Type = elevator.Type,
                        NumberOfStops = elevator.NumberOfStops,
                        NumberOfFloors = elevator.NumberOfFloors,
                        NextMaintenanceDate = elevator.NextMaintenanceDate,
                        Status = elevator.Status.ToString(),
                        ContractStatus = contract?.Status.ToString() ?? string.Empty,
                        CreatedAt = elevator.CreatedAt,
                        IsFromInstallation = contract?.IsFromInstallation ?? false
                    });

                    elevatorIndex++;
                }
            }

            return result.OrderBy(e => e.CustomerName).ThenBy(e => e.CreatedAt).ToList();
        }
    }
}

