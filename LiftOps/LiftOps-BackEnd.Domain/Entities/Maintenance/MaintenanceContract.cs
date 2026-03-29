using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Domain.Entities.Maintenance
{
    public class MaintenanceContract : BaseAuditableEntity
    {
        public Guid CustomerId { get; set; }
        // Using Installation.Customer as the shared customer entity
        public Customer Customer { get; set; } = null!;

        public string ProjectNumber { get; set; } = string.Empty; // Unique project number for maintenance
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public bool IsFromInstallation { get; set; } = false; // True if converted from installation project

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PricePerMonth { get; set; }
        public int FreeMonths { get; set; }
        
        public MaintenanceContractStatus Status { get; set; }
        public string? FrozenReason { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        
        public Guid? TechnicianId { get; set; }
        public Technician? Technician { get; set; }

        public ICollection<MaintenanceElevator> Elevators { get; set; } = new List<MaintenanceElevator>();
    }

    public enum MaintenanceContractStatus
    {
        Active,
        Pending,
        Frozen,
        Cancelled,
        Expired
    }
}
