using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface IInspectionRequestRepository : IGenericRepository<InspectionRequest>
    {
        Task<InspectionRequest?> GetInspectionWithOfferAsync(Guid id);
        Task<IReadOnlyList<InspectionRequest>> GetInspectionsByStatusAsync(InspectionStatus? status);
    }
}

