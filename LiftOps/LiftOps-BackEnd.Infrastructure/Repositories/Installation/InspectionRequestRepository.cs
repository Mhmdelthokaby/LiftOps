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
    public class InspectionRequestRepository : GenericRepository<InspectionRequest>, IInspectionRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public InspectionRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<InspectionRequest?> GetInspectionWithOfferAsync(Guid id)
        {
            return await _context.InspectionRequests
                .Include(i => i.Offer)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IReadOnlyList<InspectionRequest>> GetInspectionsByStatusAsync(InspectionStatus? status)
        {
            var query = _context.InspectionRequests
                .Include(i => i.Offer)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            return await query
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
    }
}

