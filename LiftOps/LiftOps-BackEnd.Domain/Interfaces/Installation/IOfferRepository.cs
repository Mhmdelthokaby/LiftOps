using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {
        Task<Offer?> GetOfferWithInspectionAsync(Guid id);
        Task<Offer?> GetOfferByInspectionRequestIdAsync(Guid inspectionRequestId);
        Task<IReadOnlyList<Offer>> GetOffersByStatusAsync(OfferStatus? status);
    }
}

