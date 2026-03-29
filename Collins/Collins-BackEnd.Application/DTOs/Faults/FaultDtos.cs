using System;
using System.Collections.Generic;
using Collins_BackEnd.Domain.Entities.Faults; // For enums

namespace Collins_BackEnd.Application.DTOs.Faults
{
    public class CreateFaultTicketDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ProjectAddress { get; set; } = string.Empty;
        public string? ProjectNumber { get; set; }
        public string? GoogleMapsLink { get; set; }
        public string ElevatorType { get; set; } = string.Empty;
        public string FaultDescription { get; set; } = string.Empty;
        public FaultSeverity Severity { get; set; }
    }

    public class AssignFaultTechnicianDto
    {
        public Guid TechnicianId { get; set; }
    }

    public class AddFaultPartsDto
    {
        public Guid TicketId { get; set; }
        public List<PartUsageDto> Parts { get; set; } = new List<PartUsageDto>();
    }

    public class PartUsageDto // Helper, could be shared or duplicated
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class ResolveFaultDto
    {
        public string Notes { get; set; } = string.Empty;
    }
}
