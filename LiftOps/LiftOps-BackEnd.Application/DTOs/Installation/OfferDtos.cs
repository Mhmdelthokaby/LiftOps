using System;
using System.ComponentModel.DataAnnotations;

namespace LiftOps_BackEnd.Application.DTOs.Installation
{
    public class CreateOfferDto
    {
        [Required(ErrorMessage = "Inspection request ID is required")]
        public Guid InspectionRequestId { get; set; }
        
        [Range(0.01, 99999999.99, ErrorMessage = "Installation price per unit must be greater than 0")]
        public decimal InstallationPricePerUnit { get; set; }
        
        [Range(0.01, 99999999.99, ErrorMessage = "Total installation price must be greater than 0")]
        public decimal TotalInstallationPrice { get; set; }
        
        [Required(ErrorMessage = "Estimated start date is required")]
        [DataType(DataType.Date)]
        public DateTime EstimatedStartDate { get; set; }
        
        [Required(ErrorMessage = "Estimated end date is required")]
        [DataType(DataType.Date)]
        public DateTime EstimatedEndDate { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
        // PDF will be uploaded separately if needed
    }
    
    public class UpdateOfferDto
    {
        [Range(0.01, 99999999.99, ErrorMessage = "Installation price per unit must be greater than 0")]
        public decimal? InstallationPricePerUnit { get; set; }
        
        [Range(0.01, 99999999.99, ErrorMessage = "Total installation price must be greater than 0")]
        public decimal? TotalInstallationPrice { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EstimatedStartDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EstimatedEndDate { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
    }
    
    public class ApproveOfferDto
    {
        [Required(ErrorMessage = "Offer ID is required")]
        public Guid OfferId { get; set; }
        
        [Required(ErrorMessage = "Acceptance status is required")]
        public bool IsAccepted { get; set; } // true = accept, false = reject
        
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }
    }
    
    public class UpdateOfferPdfDto
    {
        [Required(ErrorMessage = "Offer ID is required")]
        public Guid OfferId { get; set; }
        
        [Required(ErrorMessage = "PDF path is required")]
        [StringLength(500, ErrorMessage = "PDF path cannot exceed 500 characters")]
        public string OfferPdfPath { get; set; } = string.Empty;
    }
    
    public class OfferDto
    {
        public Guid Id { get; set; }
        public Guid InspectionRequestId { get; set; }
        public decimal InstallationPricePerUnit { get; set; }
        public decimal TotalInstallationPrice { get; set; }
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? OfferPdfPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
    
    public class ConvertOfferToProjectDto
    {
        public Guid OfferId { get; set; }
        // Additional fields if needed during conversion
    }
}

