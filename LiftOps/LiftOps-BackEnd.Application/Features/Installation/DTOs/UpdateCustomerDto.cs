namespace LiftOps_BackEnd.Application.Features.Installation.DTOs;

public class UpdateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ProjectNumber { get; set; }
    public string? GoogleMapsLink { get; set; }
}

