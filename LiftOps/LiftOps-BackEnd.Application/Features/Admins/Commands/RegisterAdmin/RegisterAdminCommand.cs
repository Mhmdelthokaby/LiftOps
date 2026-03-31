using LiftOps_BackEnd.Application.Common;
using LiftOps_BackEnd.Application.Features.Admins.DTOs;
using LiftOps_BackEnd.Application.Interfaces;
using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftOps_BackEnd.Application.Features.Admins.Commands.RegisterAdmin;

public record RegisterAdminCommand(RegisterAdminDto RegisterDto) : IRequest<Result>;

public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ICompanyProvisioningService _companyProvisioningService;

    public RegisterAdminCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ICompanyProvisioningService companyProvisioningService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _companyProvisioningService = companyProvisioningService;
    }

    public async Task<Result> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.RegisterDto.Email);
        if (existingUser != null) return Result.Failure("A user with this email already exists.");

        var company = await ResolveCompanyAsync(request.RegisterDto, cancellationToken);
        if (company == null)
        {
            return Result.Failure("Company not found or not active.");
        }

        var existingUsersInCompany = await _userManager.Users.CountAsync(u => u.CompanyId == company.Id, cancellationToken);

        var user = new AppUser
        {
            FullName = request.RegisterDto.Name,
            Email = request.RegisterDto.Email,
            UserName = request.RegisterDto.Email,
            PhoneNumber = request.RegisterDto.Phone,
            IsDisabled = false,
            CompanyId = company.Id
        };

        var result = await _userManager.CreateAsync(user, request.RegisterDto.Password);

        if (result.Succeeded)
        {
            var rolesToAssign = (request.RegisterDto.Roles ?? new List<string>())
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // First user in a tenant must be a manager (owner-level privileges in current role model).
            if (existingUsersInCompany == 0 && !rolesToAssign.Contains(Roles.Manager, StringComparer.OrdinalIgnoreCase))
            {
                rolesToAssign.Add(Roles.Manager);
            }

            if (rolesToAssign.Any())
            {
                // Ensure each role exists; create if missing
                foreach (var role in rolesToAssign)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                    }
                }
                await _userManager.AddToRolesAsync(user, rolesToAssign);
            }
            return Result.Success();
        }

        return Result.Failure(result.Errors.Select(e => e.Description).ToArray());
    }

    private async Task<Company?> ResolveCompanyAsync(RegisterAdminDto registerDto, CancellationToken cancellationToken)
    {
        if (registerDto.CompanyId.HasValue)
        {
            return await _companyProvisioningService.GetActiveByIdAsync(registerDto.CompanyId.Value, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(registerDto.CompanySlug))
        {
            return await _companyProvisioningService.GetActiveBySlugAsync(registerDto.CompanySlug.Trim(), cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(registerDto.CompanyName))
        {
            return await _companyProvisioningService.CreateAsync(registerDto.CompanyName.Trim(), cancellationToken);
        }

        return null;
    }
}
