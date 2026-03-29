using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Installation
{
    public class Offer : BaseAuditableEntity
    {
        public Guid InspectionRequestId { get; set; }
        public InspectionRequest InspectionRequest { get; set; } = null!;
        
        // Pricing
        public decimal InstallationPricePerUnit { get; set; }
        public decimal TotalInstallationPrice { get; set; }
        
        // Dates
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        
        // Status
        public OfferStatus Status { get; set; } = OfferStatus.WaitingForClientApproval;
        
        // Notes
        public string? Notes { get; set; }
        
        // PDF Attachment
        public string? OfferPdfPath { get; set; }
    }
}

