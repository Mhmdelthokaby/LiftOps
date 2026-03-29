using System;
using System.Collections.Generic;

namespace Collins_BackEnd.Application.DTOs.Installation
{
    public class AddElevatorDto
    {
        public Guid ProjectId { get; set; }
        public CreateElevatorDto Elevator { get; set; } = null!;
    }

    public class StartStageDto
    {
        public Guid StageId { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class TechnicianRatingDto
    {
        public Guid TechnicianId { get; set; }
        public double Rating { get; set; } // Rating value (typically 1-5)
    }

    public class CompleteStageDto
    {
        public Guid StageId { get; set; }
        public decimal? SupplyCost { get; set; }
        public string? Notes { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Price { get; set; } // Stage price
        public bool CollectPrice { get; set; } // Whether price was collected
        public int? FreeMonths { get; set; } // Free months for maintenance (only used for stage 4)
        public List<TechnicianRatingDto> TechnicianRatings { get; set; } = new List<TechnicianRatingDto>(); // Ratings for technicians assigned to this stage
    }

    public class PartSelectionDto
    {
        public Guid InventoryItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddStagePartsDto
    {
        public Guid StageId { get; set; }
        public List<PartSelectionDto> Parts { get; set; } = new List<PartSelectionDto>();
    }

    public class UpdateStageDto
    {
        public Guid StageId { get; set; }
        public List<PartSelectionDto>? Parts { get; set; } // If provided, will replace all existing parts
        public string? Notes { get; set; }
        public decimal? SupplyCost { get; set; }
        public decimal? StagePrice { get; set; } // Stage price
        public DateTime? EndDate { get; set; } // End date
        public List<Guid>? TechnicianIds { get; set; } // List of technician IDs assigned to this stage
    }
}
