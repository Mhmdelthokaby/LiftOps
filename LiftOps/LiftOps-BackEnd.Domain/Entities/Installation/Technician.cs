using LiftOps_BackEnd.Domain.Common;
using System;
using System.Collections.Generic;

namespace LiftOps_BackEnd.Domain.Entities.Installation
{
    public class Technician : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        
        // Link to AppUser for authentication
        public Guid? UserId { get; set; }
        
        // Performance & Workload
        public int TotalElevatorsInstalled { get; set; }
        public int CurrentActiveElevatorsCount { get; set; }
        public double? OverallRating { get; set; }
        
        public bool IsDisabled { get; set; }

        // Leader relationship (self-referencing)
        public Guid? LeaderId { get; set; }
        public Technician? Leader { get; set; }
        public ICollection<Technician> Subordinates { get; set; } = new List<Technician>();

        public ICollection<TechnicianAssignment> Assignments { get; set; } = new List<TechnicianAssignment>();
    }
}
