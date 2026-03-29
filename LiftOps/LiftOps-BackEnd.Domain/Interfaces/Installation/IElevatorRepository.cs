using LiftOps_BackEnd.Domain.Entities.Installation;
using System;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Domain.Interfaces.Installation
{
    public interface IElevatorRepository : IGenericRepository<Elevator>
    {
        Task<Elevator?> GetElevatorWithStagesAsync(Guid id);
    }
}
