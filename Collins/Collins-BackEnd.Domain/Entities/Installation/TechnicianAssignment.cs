using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Installation
{
    public class TechnicianAssignment : BaseAuditableEntity
    {
        public Guid TechnicianId { get; set; }
        public Technician Technician { get; set; } = null!;

        public Guid ElevatorId { get; set; }
        public Elevator Elevator { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedFinishDate { get; set; }
        public Guid AssignedBy { get; set; } // InstallationAdmin Id
    }
}
