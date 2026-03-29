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
    public class OfferRepository : GenericRepository<Offer>, IOfferRepository
    {
        private readonly ApplicationDbContext _context;

        public OfferRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Offer?> GetOfferWithInspectionAsync(Guid id)
        {
            return await _context.Offers
                .Include(o => o.InspectionRequest)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Offer?> GetOfferByInspectionRequestIdAsync(Guid inspectionRequestId)
        {
            return await _context.Offers
                .FirstOrDefaultAsync(o => o.InspectionRequestId == inspectionRequestId);
        }

        public async Task<IReadOnlyList<Offer>> GetOffersByStatusAsync(OfferStatus? status)
        {
            var query = _context.Offers
                .Include(o => o.InspectionRequest)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}

