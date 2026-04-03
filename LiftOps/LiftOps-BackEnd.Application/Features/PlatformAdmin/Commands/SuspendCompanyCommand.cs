using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record SuspendCompanyCommand(Guid CompanyId, SuspendCompanyRequest Request) : IRequest<Result>;

public class SuspendCompanyCommandHandler : IRequestHandler<SuspendCompanyCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public SuspendCompanyCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(SuspendCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (company == null)
        {
            return Result.Failure("Company not found.");
        }

        company.TenantStatus = CompanyTenantStatus.SuspendedByAdmin;
        company.SuspendedAt = DateTime.UtcNow;
        company.SuspensionReason = request.Request.Reason;
        company.IsActive = false;

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
