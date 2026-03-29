using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Application.DTOs.Installation
{
    public class CustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = "القاهرة الجديدة";
        public string ProjectNumber { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
    }

    public class ContractDto
    {
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public decimal InstallationPricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime? InstallationStartDate { get; set; }
        public DateTime? ExpectedFinishDate { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateElevatorDto
    {
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

        // Price
        public decimal Price { get; set; }
    }
    
    public class CreateProjectDto
    {
        public CustomerDto Customer { get; set; } = null!;
        public ContractDto Contract { get; set; } = null!;
        public List<CreateElevatorDto> Elevators { get; set; } = new List<CreateElevatorDto>();
    }
}
