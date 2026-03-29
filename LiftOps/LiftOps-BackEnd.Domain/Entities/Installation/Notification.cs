using LiftOps_BackEnd.Domain.Common;
using System;

namespace LiftOps_BackEnd.Domain.Entities.Installation
{
    public class Notification : BaseAuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid TargetUserId { get; set; } // e.g. InventoryAdmin
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        OutOfStock,
        StageCompleted,
        NewFault
    }
}
