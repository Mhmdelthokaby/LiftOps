using Collins_BackEnd.Domain.Common;
using System;

namespace Collins_BackEnd.Domain.Entities.Faults
{
    public class FaultSparePartUsage : BaseAuditableEntity
    {
        public Guid FaultTicketId { get; set; }
        public FaultTicket FaultTicket { get; set; } = null!;

        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal PriceAtTimeOfUsage { get; set; }
        public bool IsPaid { get; set; }
    }
}
