namespace LiftOps_BackEnd.Application.Interfaces;

public interface ICurrentTenantService
{
    Guid? CompanyId { get; }
    bool IsResolved { get; }
    string? Slug { get; }
}
