using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Infrastructure.Persistence;

namespace Collins_BackEnd.Infrastructure.Repositories.Installation
{
    public class StageRequiredPartRepository : GenericRepository<StageRequiredPart>, IStageRequiredPartRepository
    {
        public StageRequiredPartRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
