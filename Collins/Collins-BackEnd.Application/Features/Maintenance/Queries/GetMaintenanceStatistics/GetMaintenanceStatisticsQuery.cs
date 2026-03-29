using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Domain.Entities.Maintenance;
using Collins_BackEnd.Domain.Interfaces;
using Collins_BackEnd.Domain.Interfaces.Maintenance;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Queries.GetMaintenanceStatistics
{
    public class GetMaintenanceStatisticsQuery : IRequest<MaintenanceStatisticsDto>
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class GetMaintenanceStatisticsQueryHandler : IRequestHandler<GetMaintenanceStatisticsQuery, MaintenanceStatisticsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMaintenanceRepository _maintenanceRepository;

        public GetMaintenanceStatisticsQueryHandler(IUnitOfWork unitOfWork, IMaintenanceRepository maintenanceRepository)
        {
            _unitOfWork = unitOfWork;
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<MaintenanceStatisticsDto> Handle(GetMaintenanceStatisticsQuery request, CancellationToken cancellationToken)
        {
            // Get all contracts with elevators included
            var contracts = await _maintenanceRepository.GetAllContractsWithDetailsAsync();
            var visits = await _unitOfWork.Repository<MaintenanceVisit>().ListAllAsync();

            // Filter to only ACTIVE contracts
            var activeContracts = contracts
                .Where(c => c.Status == MaintenanceContractStatus.Active)
                .ToList();

            var stats = new MaintenanceStatisticsDto();

            // Calculate statistics for active contracts only
            foreach (var contract in activeContracts)
            {
                // Filter to only ACTIVE elevators (exclude Frozen and Stopped)
                var activeElevators = contract.Elevators?
                    .Where(e => e.Status == MaintenanceElevatorStatus.Active)
                    .ToList() ?? new List<MaintenanceElevator>();

                // Skip contracts with no active elevators
                if (!activeElevators.Any())
                    continue;

                // Count free vs paid projects (only active contracts with active elevators)
                if (contract.PricePerMonth == 0)
                {
                    stats.TotalFreeProjects++;
                }
                else
                {
                    stats.TotalPaidProjects++;
                }

                // Track maintenance progress for ACTIVE elevators only
                stats.TotalMaintenanceTasks += activeElevators.Count;

                // Get elevator IDs for active elevators only
                var activeElevatorIds = activeElevators.Select(e => e.Id).ToList();
                
                // Get visits for active elevators in the specified month/year
                var contractVisits = visits
                    .Where(v => activeElevatorIds.Contains(v.MaintenanceElevatorId) &&
                                v.VisitDate.Year == request.Year &&
                                v.VisitDate.Month == request.Month)
                    .ToList();

                // Count active elevators with completed maintenance
                var activeElevatorsWithDoneVisits = contractVisits
                    .Where(v => v.Status == VisitStatus.Done)
                    .Select(v => v.MaintenanceElevatorId)
                    .Distinct()
                    .Count();

                // If all active elevators have done visits, count all active elevators as completed
                if (activeElevatorsWithDoneVisits == activeElevators.Count && activeElevators.Count > 0)
                {
                    stats.CompletedMaintenanceTasks += activeElevators.Count;
                }
                else
                {
                    // Otherwise, count only active elevators with done visits
                    stats.CompletedMaintenanceTasks += activeElevatorsWithDoneVisits;
                }

                // Calculate collection for paid active contracts (not in free period)
                if (contract.PricePerMonth > 0)
                {
                    // Check if contract is in free period for the selected month
                    var startDate = contract.StartDate;
                    var targetDate = new DateTime(request.Year, request.Month, 1);
                    var yearDiff = targetDate.Year - startDate.Year;
                    var monthDiff = targetDate.Month - startDate.Month;
                    var monthsElapsed = (yearDiff * 12) + monthDiff;
                    var isInFreePeriod = monthsElapsed < contract.FreeMonths;

                    if (!isInFreePeriod)
                    {
                        // This contract should pay for the selected month
                        // Calculate based on active elevators only
                        stats.TotalMustCollect += contract.PricePerMonth;

                        // Check if all ACTIVE elevators have paid visits for this month
                        var activeElevatorsWithPaidVisits = contractVisits
                            .Where(v => v.IsPaid)
                            .Select(v => v.MaintenanceElevatorId)
                            .Distinct()
                            .Count();

                        // Contract is fully collected if all ACTIVE elevators have paid visits
                        if (activeElevatorsWithPaidVisits == activeElevators.Count)
                        {
                            stats.TotalCollected += contract.PricePerMonth;
                        }
                    }
                }
            }

            stats.TotalNotCollected = stats.TotalMustCollect - stats.TotalCollected;

            return stats;
        }
    }
}
