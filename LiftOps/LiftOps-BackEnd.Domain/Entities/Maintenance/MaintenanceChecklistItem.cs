using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Entities.Maintenance
{
    public class MaintenanceChecklistItem : BaseAuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; } // For ordering items in the list
        public bool IsActive { get; set; } = true; // Allow soft delete/disable

        // Navigation property for visits that use this checklist item
        public ICollection<MaintenanceVisitChecklistItem> VisitChecklistItems { get; set; } = new List<MaintenanceVisitChecklistItem>();
    }
}

