using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Installation
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
