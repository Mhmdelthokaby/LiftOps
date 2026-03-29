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
    public class StageTechnicianRepository : GenericRepository<StageTechnician>, IStageTechnicianRepository
    {
        private readonly ApplicationDbContext _context;

        public StageTechnicianRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<StageTechnician>> GetByStageIdAsync(Guid stageId)
        {
            return await _context.Set<StageTechnician>()
                .Include(st => st.Technician)
                .Where(st => st.StageId == stageId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<StageTechnician>> GetByTechnicianIdAsync(Guid technicianId)
        {
            return await _context.Set<StageTechnician>()
                .Include(st => st.Stage)
                .Where(st => st.TechnicianId == technicianId)
                .ToListAsync();
        }
    }
}

