using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Infrastructure.Services;

public class CompanyProvisioningService : ICompanyProvisioningService
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionLifecycleService _subscriptionLifecycleService;

    public CompanyProvisioningService(
        ApplicationDbContext context,
        ISubscriptionLifecycleService subscriptionLifecycleService)
    {
        _context = context;
        _subscriptionLifecycleService = subscriptionLifecycleService;
    }

    public async Task<Company?> GetActiveByIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, cancellationToken);
    }

    public async Task<Company?> GetActiveBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive, cancellationToken);
    }

    public async Task<Company> CreateAsync(string companyName, CancellationToken cancellationToken)
    {
        var slug = companyName.ToLowerInvariant().Replace(' ', '-');

        var company = new Company
        {
            Name = companyName,
            Slug = slug,
            IsActive = true
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync(cancellationToken);
        await _subscriptionLifecycleService.EnsureTrialSubscriptionAsync(company.Id, cancellationToken);
        return company;
    }
}
