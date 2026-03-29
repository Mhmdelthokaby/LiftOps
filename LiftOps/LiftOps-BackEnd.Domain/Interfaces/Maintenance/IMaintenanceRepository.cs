using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Maintenance
{
    public interface IMaintenanceRepository : IGenericRepository<MaintenanceContract>
    {
        // Contract methods provided by generic repo or specific ones here
        Task<IReadOnlyList<MaintenanceContract>> GetContractsByCustomerAsync(Guid customerId);
        Task<IReadOnlyList<MaintenanceContract>> GetAllContractsWithDetailsAsync();
        Task<MaintenanceContract?> GetContractByIdWithDetailsAsync(Guid contractId);
        
        // Elevator methods
        Task<MaintenanceElevator?> GetElevatorByIdAsync(Guid id);
        Task<IReadOnlyList<MaintenanceElevator>> GetElevatorsByContractAsync(Guid contractId);
        Task UpdateElevatorAsync(MaintenanceElevator elevator);
        
        // Visit methods
        Task<MaintenanceVisit?> GetVisitByIdAsync(Guid id);
        Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByElevatorAsync(Guid elevatorId);
        Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByElevatorAndMonthAsync(Guid elevatorId, int? month = null, int? year = null);
        Task<IReadOnlyList<MaintenanceVisit>> GetMonthlyScheduleAsync(int month, int year);
        Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByContractAndMonthAsync(Guid contractId, int month, int year);
        Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByDateAsync(DateTime date);
        Task<bool> HasCompletedVisitForMonthAsync(Guid elevatorId, int month, int year);
        Task AddVisitAsync(MaintenanceVisit visit);
        Task UpdateVisitAsync(MaintenanceVisit visit);
        
        // Elevator listing
        Task<IReadOnlyList<MaintenanceElevator>> GetAllElevatorsWithDetailsAsync();
    }
}
