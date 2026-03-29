using System;
using System.Collections.Generic;
using LiftOps_BackEnd.Application.DTOs.Installation; // Reuse CustomerDto

namespace LiftOps_BackEnd.Application.DTOs.Maintenance
{
    public class CreateMaintenanceContractDto
    {
        public Guid CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerMonth { get; set; }
        public int FreeMonths { get; set; }
        public Guid? TechnicianId { get; set; }
    }

    // For creating full maintenance project (similar to installation)
    public class CreateMaintenanceProjectDto
    {
        public CustomerDto Customer { get; set; } = null!;
        public MaintenanceContractDto Contract { get; set; } = null!;
        public List<CreateMaintenanceElevatorDto> Elevators { get; set; } = new List<CreateMaintenanceElevatorDto>();
    }

    public class MaintenanceContractDto
    {
        public string ProjectNumber { get; set; } = string.Empty;
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerMonth { get; set; }
        public int FreeMonths { get; set; }
        public string? Notes { get; set; }
        public Guid? TechnicianId { get; set; }
    }

    public class CreateMaintenanceElevatorDto
    {
        public string Type { get; set; } = string.Empty;
        public int NumberOfStops { get; set; }
        public int NumberOfFloors { get; set; }
        public string? Notes { get; set; }
    }

    public class AddMaintenanceElevatorDto
    {
        public Guid ContractId { get; set; } // If adding to existing
        public string Type { get; set; } = string.Empty;
        public int Stops { get; set; }
        public int Floors { get; set; }
    }

    public class ScheduleVisitDto
    {
        public Guid ElevatorId { get; set; }
        public DateTime VisitDate { get; set; }
        public Guid? TechnicianId { get; set; }
    }

    public class AssignVisitTechnicianDto
    {
        public Guid TechnicianId { get; set; }
    }

    public class CompleteVisitDto
    {
        public Guid VisitId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string? PaymentNotes { get; set; }
        public List<PartUsageDto> PartsUsed { get; set; } = new List<PartUsageDto>();
        public List<VisitChecklistItemDto> ChecklistItems { get; set; } = new List<VisitChecklistItemDto>();
    }

    public class PartUsageDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    // Checklist Item DTOs
    public class MaintenanceChecklistItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMaintenanceChecklistItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
    }

    public class UpdateMaintenanceChecklistItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }

    // Checklist item selection for visits
    public class VisitChecklistItemDto
    {
        public Guid ChecklistItemId { get; set; }
        public bool IsCompleted { get; set; } // Good/Bad status
        public string? Notes { get; set; } // Description
        public int? Count { get; set; } // Count of items (e.g., 4 wires, 1 motor)
        public decimal? Percentage { get; set; } // Percentage value (e.g., 75%, 50%)
    }

    // Query DTOs for listing maintenance contracts
    public class MaintenanceContractListDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = "القاهرة الجديدة";
        public string ProjectNumber { get; set; } = string.Empty;
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public bool IsFromInstallation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerMonth { get; set; }
        public int FreeMonths { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ElevatorCount { get; set; }
        public Guid? TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for listing maintenance elevators
    public class MaintenanceElevatorListDto
    {
        public Guid Id { get; set; }
        public string ElevatorCode { get; set; } = string.Empty; // e.g., "M1", "M2"
        public Guid ContractId { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = "القاهرة الجديدة";
        public string Type { get; set; } = string.Empty;
        public int NumberOfStops { get; set; }
        public int NumberOfFloors { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ContractStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsFromInstallation { get; set; }
    }

    // DTOs for freeze/stop operations
    public class FreezeElevatorDto
    {
        public string? Reason { get; set; }
        public DateTime? FreezeEndDate { get; set; }
    }

    public class StopElevatorDto
    {
        public string? Reason { get; set; }
    }

    public class FreezeContractDto
    {
        public string? Reason { get; set; }
        public DateTime? FreezeEndDate { get; set; }
    }

    public class StopContractDto
    {
        public string? Reason { get; set; }
    }

    // Update DTOs for editing contracts and elevators
    public class UpdateMaintenanceContractDto
    {
        public string ProjectNumber { get; set; } = string.Empty;
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string? City { get; set; } // Project city
        public string? GoogleMapsLink { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerMonth { get; set; }
        public int FreeMonths { get; set; }
        public Guid? TechnicianId { get; set; }
    }

    public class UpdateMaintenanceElevatorDto
    {
        public string Type { get; set; } = string.Empty;
        public int NumberOfStops { get; set; }
        public int NumberOfFloors { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    // DTO for detailed maintenance contract view
    public class MaintenanceContractDetailsDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = "القاهرة الجديدة";
        public string ProjectNumber { get; set; } = string.Empty;
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public bool IsFromInstallation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerMonth { get; set; }
        public int FreeMonths { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? FrozenReason { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        public Guid? TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<MaintenanceElevatorDetailsDto> Elevators { get; set; } = new List<MaintenanceElevatorDetailsDto>();
    }

    public class MaintenanceElevatorDetailsDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int NumberOfStops { get; set; }
        public int NumberOfFloors { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid? InstallationElevatorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // DTO for listing maintenance visits
    public class MaintenanceVisitListDto
    {
        public Guid Id { get; set; }
        public DateTime VisitDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? PaymentNotes { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Guid ElevatorId { get; set; }
        public string ElevatorCode { get; set; } = string.Empty;
        public Guid? TechnicianId { get; set; }
        public List<MaintenanceVisitChecklistItemDto> ChecklistItems { get; set; } = new List<MaintenanceVisitChecklistItemDto>();
    }

    public class MaintenanceVisitChecklistItemDto
    {
        public Guid ChecklistItemId { get; set; }
        public string ChecklistItemTitle { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } // Good/Bad status
        public string? Notes { get; set; } // Description
        public int? Count { get; set; } // Count of items (e.g., 4 wires, 1 motor)
        public decimal? Percentage { get; set; } // Percentage value (e.g., 75%, 50%)
    }

    // DTOs for technician assignment and viewing
    public class AssignVisitToTechnicianDto
    {
        public Guid VisitId { get; set; }
        public Guid? TechnicianId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string? Notes { get; set; }
    }

    public class TechnicianVisitDto
    {
        public Guid VisitId { get; set; }
        public Guid ElevatorId { get; set; }
        public string ElevatorCode { get; set; } = string.Empty;
        public Guid ContractId { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectNotes { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public DateTime VisitDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? PaymentNotes { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class UpdateVisitStatusDto
    {
        public Guid VisitId { get; set; }
        public string Status { get; set; } = string.Empty; // "InProgress" or "Done"
    }

    // DTO for canceling incomplete visits by date
    public class CancelVisitsByDateDto
    {
        public DateTime Date { get; set; }
    }

    // DTO for assigning technicians to contract visits for today
    public class AssignTechniciansToContractVisitsDto
    {
        public Guid ContractId { get; set; }
        public List<Guid> TechnicianIds { get; set; } = new List<Guid>();
        public DateTime AssignmentDate { get; set; }
        public string? Notes { get; set; } // Notes for the visit
    }

    // DTO for maintenance statistics (only for active contracts)
    public class MaintenanceStatisticsDto
    {
        public int TotalMaintenanceTasks { get; set; } // Total elevators in active contracts
        public int CompletedMaintenanceTasks { get; set; } // Elevators with completed maintenance for the month
        public decimal TotalMustCollect { get; set; } // Total amount that should be collected from active paid contracts
        public decimal TotalCollected { get; set; } // Total amount collected from active paid contracts
        public decimal TotalNotCollected { get; set; } // Total amount not collected
        public int TotalFreeProjects { get; set; } // Active contracts with pricePerMonth = 0
        public int TotalPaidProjects { get; set; } // Active contracts with pricePerMonth > 0
    }
}
