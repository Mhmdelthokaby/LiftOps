using LiftOps_BackEnd.Domain.Entities.Maintenance;
using LiftOps_BackEnd.Application.DTOs.Maintenance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Maintenance
{
    public interface IMaintenanceService
    {
        // Contracts & Elevators
        Task<MaintenanceContract> CreateContractAsync(MaintenanceContract contract);
        Task UpdateContractAsync(Guid contractId, UpdateMaintenanceContractDto dto);
        Task<MaintenanceContract> AddElevatorToContractAsync(Guid contractId, MaintenanceElevator elevator);
        Task UpdateElevatorAsync(Guid elevatorId, UpdateMaintenanceElevatorDto dto);
        Task<IReadOnlyList<MaintenanceContract>> GetCustomerContractsAsync(Guid customerId);
        
        // Scheduling & visits
        Task GenerateMonthlyVisitsAsync(int month, int year); // Bulk generation
        Task<MaintenanceVisit> ScheduleVisitAsync(Guid elevatorId, DateTime date, Guid? technicianId = null);
        Task AssignTechnicianToVisitAsync(Guid visitId, Guid? technicianId, string? notes = null);
        Task UpdateVisitAsync(MaintenanceVisit visit);
        Task CompleteVisitAsync(Guid visitId, string notes, List<(Guid ItemId, int Qty)> partsUsed, List<(Guid ChecklistItemId, bool IsCompleted, string? Notes, int? Count, decimal? Percentage)>? checklistItems = null, string? paymentNotes = null);
        Task<MaintenanceVisit?> GetVisitByIdAsync(Guid visitId);
        
        // Reports
        Task<IReadOnlyList<MaintenanceVisit>> GetMonthlyScheduleAsync(int month, int year);
        Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByContractAndMonthAsync(Guid contractId, int month, int year);
        Task<IReadOnlyList<MaintenanceVisit>> GetVisitsByElevatorAndMonthAsync(Guid elevatorId, int? month = null, int? year = null);
        Task<int> CancelIncompleteVisitsByDateAsync(DateTime date);
        Task UpdateVisitOrderAsync(DateTime date, List<Guid> visitIds);

        // Integration
        Task TransferElevatorToMaintenanceAsync(Guid elevatorId, int freeMonths = 6);
        
        // Project Management
        Task<IReadOnlyList<MaintenanceContract>> GetAllContractsAsync();
        Task<MaintenanceContract?> GetContractByIdAsync(Guid contractId);
        Task<bool> ProjectNumberExistsAsync(string projectNumber);
        Task<MaintenanceContract> CreateMaintenanceProjectAsync(MaintenanceContract contract, List<MaintenanceElevator> elevators);
        Task MarkVisitAsPaidAsync(Guid visitId);
        
        // Elevator Management
        Task<IReadOnlyList<MaintenanceElevator>> GetAllElevatorsAsync();
        Task<IReadOnlyList<MaintenanceElevator>> GetElevatorsByContractAsync(Guid contractId);
        Task<bool> FreezeElevatorAsync(Guid elevatorId, string? reason = null, DateTime? freezeEndDate = null);
        Task<bool> StopElevatorAsync(Guid elevatorId, string? reason = null);
        Task<bool> ActivateElevatorAsync(Guid elevatorId);
        
        // Contract Management
        Task<bool> FreezeContractAsync(Guid contractId, string? reason = null, DateTime? freezeEndDate = null);
        Task<bool> StopContractAsync(Guid contractId, string? reason = null);
        Task<bool> ActivateContractAsync(Guid contractId);
        
        // Utility
        Task EnsureContractHasProjectNumberAsync(MaintenanceContract contract);
    }
}
