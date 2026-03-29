using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Installation
{
    public class TechnicianRepository : GenericRepository<Technician>, ITechnicianRepository
    {
        public TechnicianRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IReadOnlyList<Technician>> ListAllAsync()
        {
            return await _context.Technicians
                .Include(t => t.Leader)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Technician>> GetAvailableTechniciansAsync(int maxActiveProjects = 5)
        {
            return await _context.Technicians
                .Include(t => t.Leader)
                .Where(t => !t.IsDisabled && t.CurrentActiveElevatorsCount < maxActiveProjects)
                .OrderBy(t => t.CurrentActiveElevatorsCount)
                .ToListAsync();
        }

        public async Task<Technician?> GetTechnicianWithAssignmentsAsync(Guid id)
        {
            return await _context.Technicians
                .Include(t => t.Assignments)
                .ThenInclude(a => a.Elevator)
                .ThenInclude(e => e.Project) // To see project details
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IReadOnlyList<Technician>> GetSubordinatesByLeaderIdAsync(Guid leaderId)
        {
            return await _context.Technicians
                .Where(t => t.LeaderId == leaderId)
                .ToListAsync();
        }

        public async Task<Technician?> GetTechnicianByUserIdAsync(Guid userId)
        {
            return await _context.Technicians
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }
    }
}
