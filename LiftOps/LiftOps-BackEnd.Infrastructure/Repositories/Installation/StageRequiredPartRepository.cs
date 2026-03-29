using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Infrastructure.Persistence;

namespace LiftOps_BackEnd.Infrastructure.Repositories.Installation
{
    public class StageRequiredPartRepository : GenericRepository<StageRequiredPart>, IStageRequiredPartRepository
    {
        public StageRequiredPartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
