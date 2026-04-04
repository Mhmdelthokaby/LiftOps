using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LiftOps_BackEnd.Infrastructure.Persistence;

/// <summary>
/// Ensures a platform super-admin user exists for initial SaaS console access (optional, config-driven).
/// </summary>
public static class PlatformAdminSeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        if (!configuration.GetValue("PlatformAdmin:SeedOnStartup", false))
        {
            return;
        }

        var email = configuration["PlatformAdmin:Email"]?.Trim();
        var password = configuration["PlatformAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("PlatformAdmin seeder skipped: PlatformAdmin:Email or PlatformAdmin:Password is not set.");
            return;
        }

        if (!await roleManager.RoleExistsAsync(Roles.PlatformAdmin))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.PlatformAdmin));
            logger.LogInformation("Created role {Role}.", Roles.PlatformAdmin);
        }

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            if (!await userManager.IsInRoleAsync(existing, Roles.PlatformAdmin))
            {
                await userManager.AddToRoleAsync(existing, Roles.PlatformAdmin);
                logger.LogInformation("Added {Role} to existing user {Email}.", Roles.PlatformAdmin, email);
            }

            return;
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = configuration["PlatformAdmin:FullName"]?.Trim() ?? "Platform Administrator",
            CompanyId = null,
            IsDisabled = false
        };

        var create = await userManager.CreateAsync(user, password);
        if (!create.Succeeded)
        {
            logger.LogError(
                "Platform admin user creation failed for {Email}: {Errors}",
                email,
                string.Join("; ", create.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, Roles.PlatformAdmin);
        logger.LogInformation("Seeded platform admin user {Email}.", email);
    }
}
