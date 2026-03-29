using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Installation
{
    public class StageTechnician : BaseAuditableEntity
    {
        public Guid StageId { get; set; }
        public InstallationStage Stage { get; set; } = null!;

        public Guid TechnicianId { get; set; }
        public Technician Technician { get; set; } = null!;

        public double? Rating { get; set; } // Rating given to technician for this stage (1-5 typically)
    }
}

