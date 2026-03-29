using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Interfaces.Installation
{
    public interface IInstallationProjectService
    {
        Task<InstallationProject> CreateProjectAsync(InstallationProject project);
        Task UpdateProjectAsync(UpdateProjectDto dto);
        Task<bool> ProjectNumberExistsAsync(string projectNumber);
        Task<Customer?> GetCustomerByContactAsync(string email, string phone);
        Task<int> FixDuplicateProjectNumbersAsync();
    }
}
