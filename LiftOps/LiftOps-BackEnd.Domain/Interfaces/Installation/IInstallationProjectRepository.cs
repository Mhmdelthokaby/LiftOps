using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface IInstallationProjectRepository : IGenericRepository<InstallationProject>
    {
        Task<InstallationProject?> GetProjectWithDetailsAsync(Guid id);
        Task<IReadOnlyList<InstallationProject>> GetProjectsWithStatusAsync();
    }
}
