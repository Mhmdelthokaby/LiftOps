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
        // SqlServerRetryingExecutionStrategy requires transactions to run inside CreateExecutionStrategy().ExecuteAsync.
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            Company company;
            try
            {
                var companyExists = await _db.Companies.AnyAsync(c => c.Name.ToLower() == req.CompanyName.ToLower(), cancellationToken);
                if (companyExists)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<CreateCompanyResponseDto>.Failure("A company with this name already exists.");
                }

                SubscriptionPlan? plan;
                if (req.PlanId is { } explicitPlanId && explicitPlanId != Guid.Empty)
                {
                    plan = await _db.SubscriptionPlans
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == explicitPlanId && p.IsActive, cancellationToken);
                    if (plan == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<CreateCompanyResponseDto>.Failure("Subscription plan not found or inactive.");
                    }
                }
                else
                {
                    plan = await _db.SubscriptionPlans
                        .AsNoTracking()
                        .Where(p => p.IsActive)
                        .OrderBy(p => p.MonthlyPrice)
                        .ThenBy(p => p.Name)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (plan == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<CreateCompanyResponseDto>.Failure("No active subscription plan is available. Create a plan in the admin console first.");
                    }
                }

                var slugBase = new string(req.CompanyName.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
                if (string.IsNullOrEmpty(slugBase))
                {
                    slugBase = "company";
                }

                company = new Company
                {
                    Name = req.CompanyName.Trim(),
                    Slug = slugBase + "-" + Guid.NewGuid().ToString("N")[..8],
                    IsActive = true,
                    BillingContactEmail = req.AdminEmail.Trim(),
                    TenantStatus = CompanyTenantStatus.Active,
                    PlanId = plan.Id
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

                var user = new AppUser
                {
                    UserName = req.AdminEmail.Trim(),
                    Email = req.AdminEmail.Trim(),
                    FullName = req.CompanyName.Trim(),
                    CompanyId = company.Id,
                    IsDisabled = false,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, req.Password);
                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<CreateCompanyResponseDto>.Failure(createResult.Errors.Select(e => e.Description).ToArray());
                }

                if (!await _roleManager.RoleExistsAsync(Roles.Manager))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Manager));
                }

                await _userManager.AddToRoleAsync(user, Roles.Manager);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<CreateCompanyResponseDto>.Failure($"An error occurred while creating the company: {ex.Message}");
            }

            var detail = await CompanyDetailAssembler.BuildAsync(_db, company.Id, cancellationToken);
            if (detail == null)
            {
                return Result<CreateCompanyResponseDto>.Failure("Company created but details could not be retrieved.");
            }

            return Result<CreateCompanyResponseDto>.Success(new CreateCompanyResponseDto(detail, req.Password));
        });
    }
}
