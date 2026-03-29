using System;
using System.ComponentModel.DataAnnotations;

namespace LiftOps_BackEnd.Application.DTOs.Installation
{
    public class CreateInspectionRequestDto
    {
        // Optional: Link to existing client
        public Guid? ClientId { get; set; }
        
        [Required(ErrorMessage = "Client name is required")]
        [StringLength(200, ErrorMessage = "Client name cannot exceed 200 characters")]
        public string ClientName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Client phone is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string ClientPhone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Client email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string ClientEmail { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Project address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string ProjectAddress { get; set; } = string.Empty;
        
        [Url(ErrorMessage = "Invalid Google Maps link")]
        [StringLength(500, ErrorMessage = "Google Maps link cannot exceed 500 characters")]
        public string? GoogleMapsLink { get; set; }
        
        [Range(1, 100, ErrorMessage = "Number of elevators must be between 1 and 100")]
        public int NumberOfElevatorsRequired { get; set; }
        
        [Required(ErrorMessage = "Elevator type is required")]
        [StringLength(100, ErrorMessage = "Elevator type cannot exceed 100 characters")]
        public string ElevatorType { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
    }
    
    public class UpdateInspectionTechnicalDataDto
    {
        [StringLength(50, ErrorMessage = "Shaft type cannot exceed 50 characters")]
        public string? ShaftType { get; set; } // Concrete / Brick
        
        [Range(0, 999999.99, ErrorMessage = "Shaft width must be between 0 and 999999.99")]
        public decimal? ShaftWidth { get; set; }
        
        [Range(0, 999999.99, ErrorMessage = "Shaft depth must be between 0 and 999999.99")]
        public decimal? ShaftDepth { get; set; }
        
        [Range(0, 999999.99, ErrorMessage = "Last floor height must be between 0 and 999999.99")]
        public decimal? LastFloorHeight { get; set; }
        
        [Range(0, 999999.99, ErrorMessage = "Pit depth must be between 0 and 999999.99")]
        public decimal? PitDepth { get; set; }
        
        [Range(0, 999999.99, ErrorMessage = "Travel height must be between 0 and 999999.99")]
        public decimal? TravelHeight { get; set; }
        
        [StringLength(2000, ErrorMessage = "Technical notes cannot exceed 2000 characters")]
        public string? TechnicalNotes { get; set; }
    }
    
    public class InspectionRequestDto
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ProjectAddress { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public int NumberOfElevatorsRequired { get; set; }
        public string ElevatorType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        
        // Technical Data
        public string? ShaftType { get; set; }
        public decimal? ShaftWidth { get; set; }
        public decimal? ShaftDepth { get; set; }
        public decimal? LastFloorHeight { get; set; }
        public decimal? PitDepth { get; set; }
        public decimal? TravelHeight { get; set; }
        public string? TechnicalNotes { get; set; }
        
        // Offer Info
        public OfferDto? Offer { get; set; }
        public Guid? ConvertedToProjectId { get; set; }
    }
}

