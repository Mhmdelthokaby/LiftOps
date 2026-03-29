using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using System;

namespace LiftOps_BackEnd.Domain.Entities.Emergency
{
    public class EmergencyTicket : BaseAuditableEntity
    {
        public int TicketNumber { get; set; }
        public string Project { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public EmergencyPriority Priority { get; set; }
        public EmergencyStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
        public string Contact { get; set; } = string.Empty;
        
        public Guid? AssignedTechnicianId { get; set; }
        public Technician? AssignedTechnician { get; set; }
        
        public string? Notes { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }

    public enum EmergencyPriority
    {
        Low,
        Medium,
        High
    }

    public enum EmergencyStatus
    {
        Open,
        EnRoute,
        InProgress,
        Resolved
    }
}

