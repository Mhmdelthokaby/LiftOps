using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record ActivateCompanyCommand(Guid CompanyId) : IRequest<Result>;

public class ActivateCompanyCommandHandler : IRequestHandler<ActivateCompanyCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public ActivateCompanyCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(ActivateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (company == null)
        {
            return Result.Failure("Company not found.");
        }

        company.TenantStatus = CompanyTenantStatus.Active;
        company.SuspendedAt = null;
        company.SuspensionReason = null;
        company.IsActive = true;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
