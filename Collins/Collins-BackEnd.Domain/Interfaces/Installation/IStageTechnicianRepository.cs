using Collins_BackEnd.Domain.Entities.Installation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collins_BackEnd.Domain.Interfaces.Installation
{
    public interface IStageTechnicianRepository : IGenericRepository<StageTechnician>
    {
        Task<IReadOnlyList<StageTechnician>> GetByStageIdAsync(Guid stageId);
        Task<IReadOnlyList<StageTechnician>> GetByTechnicianIdAsync(Guid technicianId);
    }
}

