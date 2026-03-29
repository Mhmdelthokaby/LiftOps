using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Entities.Maintenance
{
    public class MaintenanceVisitChecklistItem : BaseAuditableEntity
    {
        public Guid VisitId { get; set; }
        public MaintenanceVisit Visit { get; set; } = null!;

        public Guid ChecklistItemId { get; set; }
        public MaintenanceChecklistItem ChecklistItem { get; set; } = null!;

        public bool IsCompleted { get; set; } = false; // Whether this item was completed in this visit (Good/Bad status) - kept for backward compatibility
        public string? Status { get; set; } // Status: "Good", "Medium", or "Bad"
        public string? Notes { get; set; } // Optional notes/description for this specific item in this visit
        public int? Count { get; set; } // Count of items (e.g., 4 wires, 1 motor)
        public decimal? Percentage { get; set; } // Percentage value (e.g., 75%, 50%)
    }
}

