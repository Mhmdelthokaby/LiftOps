using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Maintenance
{
    public class MaintenanceElevator : BaseAuditableEntity
    {
        public Guid ContractId { get; set; }
        public MaintenanceContract Contract { get; set; } = null!;

        public string Type { get; set; } = string.Empty;
        public int NumberOfStops { get; set; }
        public int NumberOfFloors { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }
        public MaintenanceElevatorStatus Status { get; set; }
        
        // Link to original Installation Elevator if applicable (optional)
        public Guid? InstallationElevatorId { get; set; }
    }

    public enum MaintenanceElevatorStatus
    {
        Active,
        Frozen,
        Stopped
    }
}
