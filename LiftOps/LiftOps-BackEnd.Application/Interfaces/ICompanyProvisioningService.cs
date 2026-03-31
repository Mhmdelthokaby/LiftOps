using LiftOps_BackEnd.Domain.Entities;

namespace LiftOps_BackEnd.Application.Interfaces;

public interface ICompanyProvisioningService
{
    Task<Company?> GetActiveByIdAsync(Guid companyId, CancellationToken cancellationToken);
    Task<Company?> GetActiveBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<Company> CreateAsync(string companyName, CancellationToken cancellationToken);
}
