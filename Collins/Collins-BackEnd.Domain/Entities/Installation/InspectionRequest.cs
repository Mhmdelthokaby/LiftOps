using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Installation
{
    public class InspectionRequest : BaseAuditableEntity
    {
        // Client Link (nullable - only set if client exists)
        public Guid? ClientId { get; set; }
        public Customer? Client { get; set; }
        
        // Client Information (stored temporarily if client doesn't exist yet)
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        
        // Project Information
        public string ProjectAddress { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        
        // Elevator Information
        public int NumberOfElevatorsRequired { get; set; }
        public string ElevatorType { get; set; } = string.Empty;
        
        // Status
        public InspectionStatus Status { get; set; } = InspectionStatus.PendingInspection;
        
        // Admin Information
        public Guid CreatedByAdminId { get; set; } // InstallationAdmin who created this
        
        // Notes
        public string? Notes { get; set; }
        
        // Technical Inspection Data (Hoistway/Bier)
        public string? ShaftType { get; set; } // Concrete / Brick
        public decimal? ShaftWidth { get; set; }
        public decimal? ShaftDepth { get; set; }
        public decimal? LastFloorHeight { get; set; }
        public decimal? PitDepth { get; set; }
        public decimal? TravelHeight { get; set; }
        public string? TechnicalNotes { get; set; }
        
        // Navigation Properties
        public Offer? Offer { get; set; }
        public Guid? ConvertedToProjectId { get; set; } // If converted to project
        public InstallationProject? ConvertedToProject { get; set; }
    }
}

