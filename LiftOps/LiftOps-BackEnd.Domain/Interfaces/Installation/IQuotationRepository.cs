using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface IQuotationRepository : IGenericRepository<Quotation>
    {
        Task<Quotation?> GetQuotationWithProjectAsync(Guid id);
        Task<Quotation?> GetQuotationByProjectIdAsync(Guid projectId);
        Task<IReadOnlyList<Quotation>> GetQuotationsByStatusAsync(QuotationStatus? status);
    }
}

