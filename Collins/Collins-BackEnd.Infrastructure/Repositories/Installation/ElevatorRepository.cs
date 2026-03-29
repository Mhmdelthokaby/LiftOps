using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Collins_BackEnd.Infrastructure.Repositories.Installation
{
    public class ElevatorRepository : GenericRepository<Elevator>, IElevatorRepository
    {
        private readonly ApplicationDbContext _context;

        public ElevatorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Elevator?> GetElevatorWithStagesAsync(Guid id)
        {
            return await _context.Elevators
                .Include(e => e.Stages)
                .Include(e => e.Project)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
