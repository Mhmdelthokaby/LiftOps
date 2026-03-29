using System;
using Collins_BackEnd.Domain.Entities.Emergency;

namespace Collins_BackEnd.Application.DTOs.Emergency
{
    public class CreateEmergencyTicketDto
    {
        public string Project { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public EmergencyPriority Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
    }

    public class UpdateEmergencyTicketDto
    {
        public string Project { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        public EmergencyPriority Priority { get; set; }
        public EmergencyStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReportedBy { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public Guid? AssignedTechnicianId { get; set; }
        public string? Notes { get; set; }
    }

    public class AssignEmergencyTechnicianDto
    {
        public Guid TechnicianId { get; set; }
    }

    public class EmergencyTicketDto
    {
        public Guid Id { get; set; }
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
        public string? AssignedTechnicianName { get; set; }
        public string? Notes { get; set; }
        public DateTime? ResolvedDate { get; set; }
    }

    public class ResolveEmergencyTicketRequest
    {
        public string? Notes { get; set; }
    }
}

