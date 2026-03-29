using LiftOps_BackEnd.Domain.Common;
using LiftOps_BackEnd.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LiftOps_BackEnd.Infrastructure.Persistence;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        // Seed Roles
        foreach (var roleName in Roles.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        // Seed Manager
        var managerEmail = "manager@liftops.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);

        if (managerUser == null)
        {
            managerUser = new AppUser
            {
                FullName = "System Manager",
                UserName = managerEmail,
                Email = managerEmail,
                EmailConfirmed = true,
                IsDisabled = false
            };

            var result = await userManager.CreateAsync(managerUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, Roles.Manager);
            }
        }
    }
}
