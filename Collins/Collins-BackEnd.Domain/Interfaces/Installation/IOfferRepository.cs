using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Domain.Interfaces.Installation
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {
        Task<Offer?> GetOfferWithInspectionAsync(Guid id);
        Task<Offer?> GetOfferByInspectionRequestIdAsync(Guid inspectionRequestId);
        Task<IReadOnlyList<Offer>> GetOffersByStatusAsync(OfferStatus? status);
    }
}

