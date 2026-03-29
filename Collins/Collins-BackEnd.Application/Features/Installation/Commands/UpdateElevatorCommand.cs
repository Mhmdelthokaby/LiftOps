using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Domain.Entities.Installation;
using Collins_BackEnd.Domain.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Installation.Commands
{
    public class UpdateElevatorCommand : IRequest<Result<Unit>>
    {
        public Guid ElevatorId { get; set; }
        public CreateElevatorDto ElevatorDto { get; set; } = null!;
    }

    public class UpdateElevatorCommandHandler : IRequestHandler<UpdateElevatorCommand, Result<Unit>>
    {
        private readonly IElevatorService _elevatorService;
        private readonly IElevatorRepository _elevatorRepository;
        private readonly IMapper _mapper;

        public UpdateElevatorCommandHandler(
            IElevatorService elevatorService,
            IElevatorRepository elevatorRepository,
            IMapper mapper)
        {
            _elevatorService = elevatorService;
            _elevatorRepository = elevatorRepository;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(UpdateElevatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if all stages are completed
                var allStagesCompleted = await _elevatorService.AreAllStagesCompletedAsync(request.ElevatorId);
                if (allStagesCompleted)
                {
                    return Result<Unit>.Failure("Cannot update elevator details. All stages are completed. Elevator installation is finished.");
                }

                // Get existing elevator
                var existingElevator = await _elevatorRepository.GetElevatorWithStagesAsync(request.ElevatorId);
                if (existingElevator == null)
                {
                    return Result<Unit>.Failure("Elevator not found.");
                }

                // Map updated properties (preserve stages and other relationships)
                existingElevator.ElevatorType = _mapper.Map<Elevator>(request.ElevatorDto).ElevatorType;
                existingElevator.PitType = _mapper.Map<Elevator>(request.ElevatorDto).PitType;
                existingElevator.FloorsCount = request.ElevatorDto.FloorsCount > 0 ? request.ElevatorDto.FloorsCount : request.ElevatorDto.NumberOfFloors;
                existingElevator.StopsCount = request.ElevatorDto.StopsCount > 0 ? request.ElevatorDto.StopsCount : request.ElevatorDto.NumberOfStops;
                existingElevator.NumberOfFloors = request.ElevatorDto.FloorsCount > 0 ? request.ElevatorDto.FloorsCount : request.ElevatorDto.NumberOfFloors;
                existingElevator.NumberOfStops = request.ElevatorDto.StopsCount > 0 ? request.ElevatorDto.StopsCount : request.ElevatorDto.NumberOfStops;
                existingElevator.NumberOfElevators = request.ElevatorDto.NumberOfElevators;
                existingElevator.PitWidth = request.ElevatorDto.PitWidth;
                existingElevator.PitDepth = request.ElevatorDto.PitDepth;
                existingElevator.LastFloorHeight = request.ElevatorDto.LastFloorHeight;
                existingElevator.HoleDepth = request.ElevatorDto.HoleDepth;
                existingElevator.TravelLength = request.ElevatorDto.TravelLength;
                existingElevator.Notes = request.ElevatorDto.Notes;
                existingElevator.Price = request.ElevatorDto.Price;

                await _elevatorService.UpdateElevatorAsync(existingElevator);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (InvalidOperationException ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure($"An error occurred while updating the elevator: {ex.Message}");
            }
        }
    }
}

