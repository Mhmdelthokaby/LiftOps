using Collins_BackEnd.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Collins_BackEnd.Domain.Entities.Installation
{
    public class Elevator : BaseAuditableEntity
    {
        public Guid ProjectId { get; set; }
        public InstallationProject Project { get; set; } = null!;

        // Elevator Type
        [Required]
        public ElevatorType ElevatorType { get; set; }

        // Floor and Stop Information
        [Required]
        public int FloorsCount { get; set; }

        [Required]
        public int StopsCount { get; set; }

        // Legacy field - keeping for backward compatibility, but will be replaced by FloorsCount
        public int NumberOfFloors { get; set; }
        
        // Legacy field - keeping for backward compatibility, but will be replaced by StopsCount
        public int NumberOfStops { get; set; }

        // Number of elevators in this group/line item
        public int NumberOfElevators { get; set; } = 1;

        // Pit Details
        [Required]
        public PitType PitType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PitWidth { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PitDepth { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LastFloorHeight { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HoleDepth { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TravelLength { get; set; }

        // Notes
        [MaxLength(2000)]
        public string? Notes { get; set; }

        // Price
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation Properties
        public ICollection<InstallationStage> Stages { get; set; } = new List<InstallationStage>();
        public ICollection<TechnicianAssignment> TechnicianAssignments { get; set; } = new List<TechnicianAssignment>();
    }
}
