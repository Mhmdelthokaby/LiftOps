using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Entities.Maintenance;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Interfaces.Installation
{
    public interface IPdfGenerator
    {
        Task<string> GenerateStageReportAsync(InstallationStage stage);
        Task<byte[]> GenerateMaintenanceVisitReportAsync(MaintenanceVisit visit);
    }
}
