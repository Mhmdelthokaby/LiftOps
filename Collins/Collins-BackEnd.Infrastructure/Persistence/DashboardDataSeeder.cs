using Collins_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Collins_BackEnd.Infrastructure.Persistence;

public static class DashboardDataSeeder
{
    /// <summary>
    /// Seed data method - currently empty as all seed data has been removed.
    /// Only manager/admin seeding remains in AdminSeeder.
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed data has been removed - use AdminSeeder for manager/admin accounts only
        await Task.CompletedTask;
    }
}
