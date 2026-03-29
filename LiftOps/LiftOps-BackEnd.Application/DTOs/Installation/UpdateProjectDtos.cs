using System;

namespace LiftOps_BackEnd.Application.DTOs.Installation
{
    public class UpdateProjectDto
    {
        public Guid ProjectId { get; set; }
        public UpdateCustomerDto Customer { get; set; } = null!;
        public UpdateContractDto Contract { get; set; } = null!;
    }

    public class UpdateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = "القاهرة الجديدة";
        public string ProjectNumber { get; set; } = string.Empty;
        public string? GoogleMapsLink { get; set; }
    }

    public class UpdateContractDto
    {
        public string? ProjectAddress { get; set; } // Project-specific address (can differ from customer address)
        public string City { get; set; } = "القاهرة الجديدة";
        public string? GoogleMapsLink { get; set; }
        public decimal InstallationPricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime? InstallationStartDate { get; set; }
        public DateTime? ExpectedFinishDate { get; set; }
        public string? Notes { get; set; }
    }
}

