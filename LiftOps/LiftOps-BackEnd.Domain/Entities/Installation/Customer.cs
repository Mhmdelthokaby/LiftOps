using LiftOps_BackEnd.Domain.Common;
using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Domain.Entities.Installation
{
    public class Customer : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = "القاهرة الجديدة";
        public string ProjectNumber { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
        
        // Status
        public CustomerStatus Status { get; set; } = CustomerStatus.Approved;
        
        // Navigation Properties
        public ICollection<InstallationProject> Projects { get; set; } = new List<InstallationProject>();
    }
}
