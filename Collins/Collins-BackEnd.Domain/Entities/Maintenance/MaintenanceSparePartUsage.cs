using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Maintenance
{
    public class MaintenanceSparePartUsage : BaseAuditableEntity
    {
        public Guid MaintenanceVisitId { get; set; }
        public MaintenanceVisit MaintenanceVisit { get; set; } = null!;

        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal PriceAtTimeOfUsage { get; set; }
        public bool IsPaid { get; set; }
    }
}
