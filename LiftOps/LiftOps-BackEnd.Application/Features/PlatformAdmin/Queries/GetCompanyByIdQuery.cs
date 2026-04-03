using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Queries;

public record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyDetailDto>>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<CompanyDetailDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDetailDto>.Failure("Company not found.");
        }

        var sub = await _db.Subscriptions.IgnoreQueryFilters()
            .AsNoTracking()
            .Include(s => s.Plan)
            .Where(s => s.CompanyId == company.Id)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        SubscriptionDto? subDto = null;
        PlanLimitsDto? limits = null;

        if (sub != null)
        {
            var plan = sub.Plan;
            subDto = new SubscriptionDto(
                sub.Id,
                sub.CompanyId,
                sub.PlanId,
                plan.Name,
                PlatformAdminMapper.ToSubscriptionStatusString(sub.Status, sub.CurrentPeriodEnd),
                sub.CurrentPeriodStart,
                sub.CurrentPeriodEnd,
                PlatformAdminMapper.ToBillingCycleString(sub.BillingCycle));

            limits = new PlanLimitsDto(
                plan.MaxUsers,
                plan.MaxElevators,
                plan.MaxMaintenanceContracts,
                plan.MaxInstallationProjects);
        }

        var dto = new CompanyDetailDto(
            company.Id,
            company.Name,
            company.BillingContactEmail,
            company.ContactPhone,
            PlatformAdminMapper.ToCompanyStatusString(company),
            company.CreatedAt,
            subDto,
            limits);

        return Result<CompanyDetailDto>.Success(dto);
    }
}
