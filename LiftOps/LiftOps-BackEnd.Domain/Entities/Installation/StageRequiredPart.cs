using LiftOps_BackEnd.Domain.Common;
using System;

namespace LiftOps_BackEnd.Domain.Entities.Installation
{
    public class StageRequiredPart : BaseAuditableEntity
    {
        public Guid StageId { get; set; }
        public InstallationStage Stage { get; set; } = null!;

        public Guid InventoryItemId { get; set; }
        // Navigation to InventoryItem (in existing Entities folder)
        // We need to check exact namespace of InventoryItem. Assuming LiftOps_BackEnd.Domain.Entities.
        public InventoryItem InventoryItem { get; set; } = null!;

        public int Quantity { get; set; }
        public bool IsOutOfStock { get; set; }
    }
}
