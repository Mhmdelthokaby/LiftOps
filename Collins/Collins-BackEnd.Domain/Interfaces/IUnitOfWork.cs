using Collins_BackEnd.Domain.Common;

namespace Collins_BackEnd.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> Complete();
}
