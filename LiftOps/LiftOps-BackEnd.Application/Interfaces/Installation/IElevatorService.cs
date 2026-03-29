using LiftOps_BackEnd.Domain.Entities.Installation;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Interfaces.Installation
{
    public interface IElevatorService
    {
        Task<Elevator> AddElevatorAsync(Elevator elevator);
        Task<Elevator> UpdateElevatorAsync(Elevator elevator);
        Task<bool> AreAllStagesCompletedAsync(Guid elevatorId);
    }
}
