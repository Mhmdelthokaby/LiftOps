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
    public class QuotationRepository : GenericRepository<Quotation>, IQuotationRepository
    {
        private readonly ApplicationDbContext _context;

        public QuotationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Quotation?> GetQuotationWithProjectAsync(Guid id)
        {
            return await _context.Quotations
                .Include(q => q.Project)
                    .ThenInclude(p => p.Customer)
                .Include(q => q.Attachments)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quotation?> GetQuotationByProjectIdAsync(Guid projectId)
        {
            return await _context.Quotations
                .Include(q => q.Attachments)
                .FirstOrDefaultAsync(q => q.ProjectId == projectId);
        }

        public async Task<IReadOnlyList<Quotation>> GetQuotationsByStatusAsync(QuotationStatus? status)
        {
            var query = _context.Quotations
                .Include(q => q.Project)
                    .ThenInclude(p => p.Customer)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(q => q.Status == status.Value);
            }

            return await query
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }
    }
}

