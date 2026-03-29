using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Application.DTOs.Installation
{
    // DTO for creating an inspection project
    public class CreateInspectionProjectDto
    {
        // Customer information (if new customer)
        public Guid? CustomerId { get; set; } // If existing customer
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        
        // Project information
        public string ProjectAddress { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        
        // Required pit fields
        public string PitType { get; set; } = string.Empty; // Concrete / Brick
        public decimal PitWidth { get; set; }
        public decimal PitDepth { get; set; }
        public decimal LastFloorHeight { get; set; }
        public decimal HoleDepth { get; set; }
        public decimal TravelLength { get; set; }
        
        // Notes
        public string? Notes { get; set; }
    }
    
    // DTO for inspection project list item
    public class InspectionProjectDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string ProjectAddress { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public string ProjectStatus { get; set; } = string.Empty;
        public string PitType { get; set; } = string.Empty;
        public decimal PitWidth { get; set; }
        public decimal PitDepth { get; set; }
        public decimal LastFloorHeight { get; set; }
        public decimal HoleDepth { get; set; }
        public decimal TravelLength { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public QuotationDto? Quotation { get; set; }
    }
    
    // DTO for creating a quotation
    public class CreateQuotationDto
    {
        public Guid ProjectId { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string? DurationNotes { get; set; }
        public string? Notes { get; set; }
        public List<QuotationAttachmentDto>? Attachments { get; set; }
    }
    
    // DTO for quotation
    public class QuotationDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string? DurationNotes { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<QuotationAttachmentDto> Attachments { get; set; } = new List<QuotationAttachmentDto>();
        public DateTime CreatedAt { get; set; }
    }
    
    // DTO for quotation attachment
    public class QuotationAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
    
    // DTO for approving/rejecting quotation
    public class ApproveRejectQuotationDto
    {
        public Guid QuotationId { get; set; }
        public string? Notes { get; set; }
    }
}

