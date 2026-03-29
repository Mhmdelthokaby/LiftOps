using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Application.DTOs.Installation
{
    public class InstallationProjectDto
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
        public DateTime ContractDate { get; set; }
        public decimal InstallationPricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? InstallationStartDate { get; set; }
        public DateTime? ExpectedFinishDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Unknown"; // Aggregate status
        public List<ElevatorDto> Elevators { get; set; } = new List<ElevatorDto>();
    }

    public class ElevatorDto
    {
        public Guid Id { get; set; }
        
        // Elevator Type
        public string ElevatorType { get; set; } = string.Empty; // "WithMachineRoom", "MachineRoomLess", "Hydraulic"
        
        // Floor and Stop Information
        public int FloorsCount { get; set; }
        public int StopsCount { get; set; }
        
        // Legacy fields for backward compatibility
        public int NumberOfFloors { get; set; }
        public int NumberOfStops { get; set; }
        public int NumberOfElevators { get; set; } = 1;
        
        // Pit Details
        public string PitType { get; set; } = string.Empty; // "Concrete", "Brick"
        public decimal PitWidth { get; set; }
        public decimal PitDepth { get; set; }
        public decimal LastFloorHeight { get; set; }
        public decimal HoleDepth { get; set; }
        public decimal TravelLength { get; set; }
        
        // Notes
        public string? Notes { get; set; }
        
        // Price - This is the TOTAL fixed price of the elevator (never changes)
        public decimal Price { get; set; }
        
        // Computed properties for payment tracking
        // PaidAmount: Sum of all completed stage prices
        public decimal PaidAmount { get; set; }
        
        // RemainingAmount: Price - PaidAmount
        public decimal RemainingAmount { get; set; }
        
        // Stages
        public List<InstallationStageDto> Stages { get; set; } = new List<InstallationStageDto>();
    }

    public class StageRequiredPartDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public string InventoryItemName { get; set; } = string.Empty;
        public string InventoryItemNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsOutOfStock { get; set; }
    }

    public class StageTechnicianDto
    {
        public Guid Id { get; set; }
        public Guid TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
    }

    public class InstallationStageDto
    {
        public Guid Id { get; set; }
        public int StageNumber { get; set; }
        public string Status { get; set; } = string.Empty; // Enum string
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? StagePrice { get; set; }
        public bool IsPriceCollected { get; set; }
        public string? PdfPath { get; set; }
        public string? Notes { get; set; }
        public decimal? SupplyCost { get; set; }
        public Guid? StageAdminId { get; set; } // Legacy field (kept for backward compatibility)
        public List<StageRequiredPartDto> RequiredParts { get; set; } = new List<StageRequiredPartDto>();
        public List<StageTechnicianDto> Technicians { get; set; } = new List<StageTechnicianDto>();
    }
    
    public class NotificationDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
