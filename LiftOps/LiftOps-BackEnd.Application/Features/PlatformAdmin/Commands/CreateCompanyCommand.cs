using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.PlatformAdmin.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Entities.Subscription;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.PlatformAdmin.Commands;

public record CreateCompanyCommand(CreateCompanyRequest Request) : IRequest<Result<CreateCompanyResponseDto>>;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CreateCompanyResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateCompanyCommandHandler(
        IApplicationDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<CreateCompanyResponseDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var plan = await _db.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == req.PlanId, cancellationToken);

        if (plan == null)
        {
            return Result<CreateCompanyResponseDto>.Failure("Plan not found.");
        }

        var slugBase = new string(req.Name.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
        if (string.IsNullOrEmpty(slugBase))
        {
            slugBase = "company";
        }

        var company = new Company
        {
            Name = req.Name.Trim(),
            Slug = slugBase + "-" + Guid.NewGuid().ToString("N")[..8],
            IsActive = true,
            BillingContactEmail = req.ContactEmail.Trim(),
            TenantStatus = CompanyTenantStatus.Active
        };

        _db.Companies.Add(company);
        await _db.SaveChangesAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var trialDays = Math.Max(0, plan.TrialDays);
        var subscription = new Subscription
        {
            CompanyId = company.Id,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Trial,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = now.AddDays(trialDays == 0 ? 14 : trialDays),
            BillingCycle = SubscriptionBillingCycle.Monthly
        };
        _db.Subscriptions.Add(subscription);
        await _db.SaveChangesAsync(cancellationToken);

        var password = "Aa1!" + Guid.NewGuid().ToString("N")[..12];
        var user = new AppUser
        {
            UserName = req.ContactEmail.Trim(),
            Email = req.ContactEmail.Trim(),
            FullName = req.Name.Trim(),
            CompanyId = company.Id,
            IsDisabled = false,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return Result<CreateCompanyResponseDto>.Failure(createResult.Errors.Select(e => e.Description).ToArray());
        }

        if (!await _roleManager.RoleExistsAsync(Roles.Manager))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Manager));
        }

        await _userManager.AddToRoleAsync(user, Roles.Manager);

        var detail = await BuildCompanyDetailAsync(company.Id, cancellationToken);
        if (detail == null)
        {
            return Result<CreateCompanyResponseDto>.Failure("Company was created but could not be loaded.");
        }

        return Result<CreateCompanyResponseDto>.Success(new CreateCompanyResponseDto(detail, password));
    }

    private async Task<CompanyDetailDto?> BuildCompanyDetailAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var company = await _db.Companies.IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);

        if (company == null)
        {
            return null;
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

        return new CompanyDetailDto(
            company.Id,
            company.Name,
            company.BillingContactEmail,
            company.ContactPhone,
            PlatformAdminMapper.ToCompanyStatusString(company),
            company.CreatedAt,
            subDto,
            limits);
    }
}
