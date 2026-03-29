using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities.Installation; // For Technician
using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Domain.Entities.Faults
{
    public class FaultTicket : BaseAuditableEntity
    {
        // Auto-generated ticket number logic (handled in service or flow)
        public string TicketNumber { get; set; } = string.Empty;

        // Flattened customer info for quick access or independence
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ProjectAddress { get; set; } = string.Empty;
        public string? ProjectNumber { get; set; }
        public string? GoogleMapsLink { get; set; }
        public string ElevatorType { get; set; } = string.Empty;

        public string FaultDescription { get; set; } = string.Empty;
        public FaultSeverity Severity { get; set; }
        
        public DateTime FaultDate { get; set; }
        // Time can be part of DateTime or string. Storing separate as requested or just assume DateTime has both.
        // Will use DateTime for simplicity.
        
        public TicketStatus Status { get; set; }
        
        public Guid? AssignedTechnicianId { get; set; }
        public Technician? AssignedTechnician { get; set; }

        public string? Notes { get; set; }
        public DateTime? ResolvedDate { get; set; }

        public ICollection<FaultSparePartUsage> SpareParts { get; set; } = new List<FaultSparePartUsage>();
    }

    public enum FaultSeverity
    {
        Low,
        Medium,
        High
    }

    public enum TicketStatus
    {
        Pending,
        InProgress,
        Done,
        Cancelled
    }
}
