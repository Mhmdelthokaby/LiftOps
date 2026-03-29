using LiftOps_BackEnd.Domain.Entities.Faults;
using LiftOps_BackEnd.Domain.Interfaces.Faults;
using LiftOps_BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Faults
{
    public class FaultRepository : GenericRepository<FaultTicket>, IFaultRepository
    {
        public FaultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<FaultTicket>> GetOpenTicketsAsync()
        {
            return await _context.FaultTickets
                .Where(t => t.Status != TicketStatus.Done && t.Status != TicketStatus.Cancelled)
                .Include(t => t.AssignedTechnician)
                .OrderBy(t => t.FaultDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<FaultTicket>> GetTicketsByTechnicianAsync(System.Guid technicianId)
        {
            return await _context.FaultTickets
                .Where(t => t.AssignedTechnicianId == technicianId)
                 .Include(t => t.AssignedTechnician)
                .OrderByDescending(t => t.FaultDate)
                .ToListAsync();
        }
    }
}
