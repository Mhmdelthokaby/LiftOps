using Collins_BackEnd.Domain.Common;
using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Domain.Entities.Installation
{
    public class Quotation : BaseAuditableEntity
    {
        public Guid ProjectId { get; set; }
        public InstallationProject Project { get; set; } = null!;
        
        // Pricing
        public decimal Price { get; set; }
        
        // Duration (in days or as a description)
        public int DurationDays { get; set; }
        public string? DurationNotes { get; set; }
        
        // Notes
        public string? Notes { get; set; }
        
        // Attachments (stored as file paths)
        public string? AttachmentPath { get; set; }
        public ICollection<QuotationAttachment> Attachments { get; set; } = new List<QuotationAttachment>();
        
        // Status
        public QuotationStatus Status { get; set; } = QuotationStatus.Pending;
    }
    
    public class QuotationAttachment : BaseAuditableEntity
    {
        public Guid QuotationId { get; set; }
        public Quotation Quotation { get; set; } = null!;
        
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
    
    public enum QuotationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Cancelled = 3
    }
}

