using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Installation
{
    public class InstallationStageRepository : GenericRepository<InstallationStage>, IInstallationStageRepository
    {
        private readonly ApplicationDbContext _context;

        public InstallationStageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<InstallationStage?> GetStageWithPartsAsync(Guid id)
        {
            return await _context.InstallationStages
                .Include(s => s.RequiredParts)
                    .ThenInclude(rp => rp.InventoryItem)
                .Include(s => s.Technicians)
                    .ThenInclude(st => st.Technician)
                .Include(s => s.Elevator)
                    .ThenInclude(e => e.Project) // Needed for context in report
                        .ThenInclude(p => p.Customer) // Needed for report
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
