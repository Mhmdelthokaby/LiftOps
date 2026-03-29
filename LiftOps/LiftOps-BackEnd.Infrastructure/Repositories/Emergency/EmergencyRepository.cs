using LiftOps_BackEnd.Domain.Entities.Emergency;
using LiftOps_BackEnd.Domain.Interfaces.Emergency;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Emergency
{
    public class EmergencyRepository : GenericRepository<EmergencyTicket>, IEmergencyRepository
    {
        public EmergencyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<EmergencyTicket>> GetOpenTicketsAsync()
        {
            return await _context.EmergencyTickets
                .Where(t => t.Status != EmergencyStatus.Resolved)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.ReportedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<EmergencyTicket>> GetTicketsByStatusAsync(EmergencyStatus status)
        {
            return await _context.EmergencyTickets
                .Where(t => t.Status == status)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.ReportedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<EmergencyTicket>> GetTicketsByTechnicianAsync(Guid technicianId)
        {
            return await _context.EmergencyTickets
                .Where(t => t.AssignedTechnicianId == technicianId)
                .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.ReportedAt)
                .ToListAsync();
        }

        public async Task<int> GetNextTicketNumberAsync()
        {
            var lastTicket = await _context.EmergencyTickets
                .OrderByDescending(t => t.TicketNumber)
                .FirstOrDefaultAsync();
            
            return lastTicket == null ? 1000 : lastTicket.TicketNumber + 1;
        }
    }
}

