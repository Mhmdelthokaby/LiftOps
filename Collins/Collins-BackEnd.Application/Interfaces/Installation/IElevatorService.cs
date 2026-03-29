using Collins_BackEnd.Domain.Entities.Installation;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Interfaces.Installation
{
    public interface IElevatorService
    {
        Task<Elevator> AddElevatorAsync(Elevator elevator);
        Task<Elevator> UpdateElevatorAsync(Elevator elevator);
        Task<bool> AreAllStagesCompletedAsync(Guid elevatorId);
    }
}
