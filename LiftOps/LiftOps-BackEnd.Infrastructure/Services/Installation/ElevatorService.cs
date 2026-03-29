using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Domain.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Infrastructure.Services.Installation
{
    public class ElevatorService : IElevatorService
    {
        private readonly IElevatorRepository _elevatorRepository;
        private readonly IInstallationStageRepository _stageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ElevatorService(
            IElevatorRepository elevatorRepository, 
            IInstallationStageRepository stageRepository,
            IUnitOfWork unitOfWork)
        {
            _elevatorRepository = elevatorRepository;
            _stageRepository = stageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Elevator> AddElevatorAsync(Elevator elevator)
        {
            // Automatically create 4 stages
            var stages = new List<InstallationStage>();
            for (int i = 1; i <= 4; i++)
            {
                stages.Add(new InstallationStage
                {
                    ElevatorId = elevator.Id,
                    StageNumber = i,
                    Status = StageStatus.Pending
                });
            }
            // elevator.Stages = stages; // Can't set ICollection directly usually or if initialized
            // Better to add them via repository if elevator is new?
            // If elevator is new, cascading insert handles it IF added to collection.
            foreach (var s in stages) elevator.Stages.Add(s);

            _elevatorRepository.Add(elevator);
            await _unitOfWork.Complete();
            return elevator;
        }

        public async Task<bool> AreAllStagesCompletedAsync(Guid elevatorId)
        {
            var elevator = await _elevatorRepository.GetElevatorWithStagesAsync(elevatorId);
            if (elevator == null || elevator.Stages == null || elevator.Stages.Count == 0)
                return false;

            // Check if all 4 stages exist and all are Success
            return elevator.Stages.Count == 4 && 
                   elevator.Stages.All(s => s.Status == StageStatus.Success);
        }

        public async Task<Elevator> UpdateElevatorAsync(Elevator elevator)
        {
            // Check if all stages are completed - if so, prevent updates
            var allStagesCompleted = await AreAllStagesCompletedAsync(elevator.Id);
            if (allStagesCompleted)
            {
                throw new InvalidOperationException("Cannot update elevator details. All stages are completed. Elevator installation is finished.");
            }

            _elevatorRepository.Update(elevator);
            await _unitOfWork.Complete();
            return elevator;
        }
    }
}
