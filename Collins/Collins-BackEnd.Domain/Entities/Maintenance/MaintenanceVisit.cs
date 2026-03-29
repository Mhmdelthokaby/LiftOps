using Collins_BackEnd.Domain.Common;
using Collins_BackEnd.Domain.Entities.Installation; // For Technician
using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Domain.Entities.Maintenance
{
    public class MaintenanceVisit : BaseAuditableEntity
    {
        public Guid MaintenanceElevatorId { get; set; }
        public MaintenanceElevator MaintenanceElevator { get; set; } = null!;

        public DateTime VisitDate { get; set; }
        
        public Guid? TechnicianId { get; set; }
        public Technician? Technician { get; set; }

        public VisitStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? PaymentNotes { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int DisplayOrder { get; set; } = 0; // For custom ordering of visits on the same date

        public ICollection<MaintenanceSparePartUsage> SpareParts { get; set; } = new List<MaintenanceSparePartUsage>();
        public ICollection<MaintenanceVisitChecklistItem> ChecklistItems { get; set; } = new List<MaintenanceVisitChecklistItem>();
    }

    public enum VisitStatus
    {
        Pending,
        InProgress,
        Done,
        Frozen,
        Cancelled
    }
}
