using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record UpdateCompanyCommand(Guid CompanyId, UpdateCompanyRequest Request) : IRequest<Result>;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateCompanyCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == command.CompanyId, cancellationToken);

        if (company == null || company.IsDeleted)
        {
            return Result.Failure("Company not found or has been deactivated.");
        }

        var req = command.Request;
        company.Name = req.Name.Trim();
        company.IsActive = req.IsActive;

        if (req.PlanId.HasValue && req.PlanId != Guid.Empty)
        {
            var plan = await _db.SubscriptionPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == req.PlanId && p.IsActive, cancellationToken);
            
            if (plan != null)
            {
                company.PlanId = plan.Id;

                var subscription = await _db.Subscriptions.IgnoreQueryFilters()
                    .Where(s => s.CompanyId == company.Id)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (subscription != null && subscription.PlanId != plan.Id)
                {
                    subscription.PlanId = plan.Id;
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
