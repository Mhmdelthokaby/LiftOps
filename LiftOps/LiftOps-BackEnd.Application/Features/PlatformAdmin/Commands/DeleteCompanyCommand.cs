using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

/// <summary>Soft delete: marks company inactive and hidden; does not remove tenant rows.</summary>
public record DeleteCompanyCommand(Guid CompanyId) : IRequest<Result>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteCompanyCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(DeleteCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == command.CompanyId, cancellationToken);

        if (company == null)
        {
            return Result.Failure("Company not found.");
        }

        if (company.IsDeleted)
        {
            return Result.Success();
        }

        company.IsDeleted = true;
        company.IsActive = false;
        company.TenantStatus = CompanyTenantStatus.Deleted;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
