using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface IInstallationStageRepository : IGenericRepository<InstallationStage>
    {
        Task<InstallationStage?> GetStageWithPartsAsync(Guid id);
    }
}
