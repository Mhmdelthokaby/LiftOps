using LiftOps_BackEnd.Domain.Common;
using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Domain.Entities.Installation
{
    public class InstallationProject : BaseAuditableEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid InstallationAdminId { get; set; }
        // We can link to AppUser if we reference Entites.Identity or similar, but often it's loosely coupled via Guid. 
        // Assuming loose coupling or we'll add AppUser navigation if needed. 
        // For now adhering to plan: "InstallationAdminId (FK to Admin/User)"

        // Contract Details
        public string ProjectNumber { get; set; } = string.Empty; // Unique project number
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public decimal InstallationPricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime? InstallationStartDate { get; set; }
        public DateTime? ExpectedFinishDate { get; set; }
        public string? Notes { get; set; }
        
        // Project Status
        public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.UnderInspectionAndQuotation;
        
        // Technical Inspection Data (Hoistway/Bier)
        public string? ShaftType { get; set; } // Concrete / Brick (pitType)
        public decimal? ShaftWidth { get; set; } // pitWidth
        public decimal? ShaftDepth { get; set; } // pitDepth
        public decimal? LastFloorHeight { get; set; }
        public decimal? PitDepth { get; set; }
        public decimal? HoleDepth { get; set; } // Additional hole depth measurement
        public decimal? TravelHeight { get; set; } // travelLength
        public string? TechnicalNotes { get; set; }
        
        // Reference to Inspection Request if converted from inspection
        public Guid? ConvertedFromInspectionId { get; set; }
        public InspectionRequest? ConvertedFromInspection { get; set; }
        
        // Reference to Quotation
        public Guid? QuotationId { get; set; }
        public Quotation? Quotation { get; set; }

        public ICollection<Elevator> Elevators { get; set; } = new List<Elevator>();
    }
}
