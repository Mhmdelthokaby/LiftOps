using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> Complete();
}
