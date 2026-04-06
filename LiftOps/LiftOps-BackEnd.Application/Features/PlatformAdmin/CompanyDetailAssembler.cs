using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin;

internal static class CompanyDetailAssembler
{
    public static async Task<CompanyDetailDto?> BuildAsync(
        IApplicationDbContext db,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var company = await db.Companies.IgnoreQueryFilters()
            .AsNoTracking()
            .Include(c => c.Plan)
            .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);

        if (company == null)
        {
            return null;
        }

        var sub = await db.Subscriptions.IgnoreQueryFilters()
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

        var defaultAdmin = await (
            from u in db.Users.IgnoreQueryFilters()
            join ur in db.UserRoles on u.Id equals ur.UserId
            join role in db.Roles on ur.RoleId equals role.Id
            where u.CompanyId == company.Id && role.Name == Roles.Manager
            orderby u.CreatedAt
            select new { u.FullName, u.Email, u.PhoneNumber }).FirstOrDefaultAsync(cancellationToken);

        var userCount = await db.Users.IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(u => u.CompanyId == company.Id, cancellationToken);

        var elevCount = await db.Elevators.IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(e => e.CompanyId == company.Id, cancellationToken);
        var maintElev = await db.MaintenanceElevators.IgnoreQueryFilters()
            .AsNoTracking()
            .CountAsync(me => me.CompanyId == company.Id, cancellationToken);

        return new CompanyDetailDto(
            company.Id,
            company.Name,
            company.BillingContactEmail,
            company.ContactPhone,
            PlatformAdminMapper.ToCompanyStatusString(company),
            company.IsActive,
            company.IsDeleted,
            company.Plan?.Name,
            company.PlanId,
            company.CreatedAt,
            defaultAdmin?.FullName,
            defaultAdmin?.Email,
            defaultAdmin?.PhoneNumber,
            userCount,
            elevCount + maintElev,
            subDto,
            limits);
    }
}
