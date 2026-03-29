using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Domain.Interfaces.Installation
{
    public interface IInspectionRequestRepository : IGenericRepository<InspectionRequest>
    {
        Task<InspectionRequest?> GetInspectionWithOfferAsync(Guid id);
        Task<IReadOnlyList<InspectionRequest>> GetInspectionsByStatusAsync(InspectionStatus? status);
    }
}

