using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Repositories.Installation
{
    public class InstallationProjectRepository : GenericRepository<InstallationProject>, IInstallationProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public InstallationProjectRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<InstallationProject?> GetProjectWithDetailsAsync(Guid id)
        {
            // Include Customer, Elevators, and Elevators.Stages
            return await _context.InstallationProjects
                .Include(p => p.Customer)
                .Include(p => p.Elevators)
                    .ThenInclude(e => e.Stages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<InstallationProject>> GetProjectsWithStatusAsync()
        {
            // For the list view, we might not need deep stages, but maybe we do for status aggregation.
            // Plan says "returns list with status aggregates".
            // So we include everything to calculate status in memory or select projected DTOs later (but Repo returns Entities).
            try
            {
                return await _context.InstallationProjects
                    .Include(p => p.Customer)
                    .Include(p => p.Elevators)
                        .ThenInclude(e => e.Stages)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Message.Contains("Invalid column name"))
            {
                // If migration hasn't been applied yet, try to query without the new columns
                // This is a temporary workaround until migration is applied
                throw new InvalidOperationException(
                    "Database migration has not been applied. Please run: " +
                    "dotnet ef database update --project Collins-BackEnd.Infrastructure --startup-project Collins-BackEnd.API " +
                    "or use Package Manager Console: Update-Database -Project Collins-BackEnd.Infrastructure -StartupProject Collins-BackEnd.API", 
                    ex);
            }
        }
    }
}
