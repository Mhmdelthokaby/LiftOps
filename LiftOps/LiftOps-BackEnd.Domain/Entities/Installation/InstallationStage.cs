using LiftOps_BackEnd.Domain.Common;
using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Domain.Entities.Installation
{
    public enum StageStatus
    {
        Pending,
        InProgress,
        Success
    }

    public class InstallationStage : BaseAuditableEntity
    {
        public Guid ElevatorId { get; set; }
        public Elevator Elevator { get; set; } = null!;

        public int StageNumber { get; set; } // 1-4
        public StageStatus Status { get; set; } = StageStatus.Pending;
        
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public Guid? StageAdminId { get; set; }
        public decimal? SupplyCost { get; set; }
        public decimal? StagePrice { get; set; } // Price for this stage
        public bool IsPriceCollected { get; set; } // Whether price was collected
        public string? Notes { get; set; }
        
        public string? PdfPath { get; set; }

        public ICollection<StageRequiredPart> RequiredParts { get; set; } = new List<StageRequiredPart>();
        public ICollection<StageTechnician> Technicians { get; set; } = new List<StageTechnician>();
    }
}
