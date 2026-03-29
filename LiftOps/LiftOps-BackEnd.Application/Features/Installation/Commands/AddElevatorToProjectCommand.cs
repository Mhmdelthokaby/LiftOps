using LiftOps_BackEnd.Application.DTOs.Installation;
using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Domain.Entities.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Installation.Commands
{
    public class AddElevatorToProjectCommand : IRequest<Result<Guid>>
    {
        public AddElevatorDto ElevatorDto { get; set; } = null!;
    }

    public class AddElevatorToProjectCommandHandler : IRequestHandler<AddElevatorToProjectCommand, Result<Guid>>
    {
        private readonly IElevatorService _elevatorService;
        private readonly IMapper _mapper;

        public AddElevatorToProjectCommandHandler(IElevatorService elevatorService, IMapper mapper)
        {
            _elevatorService = elevatorService;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(AddElevatorToProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var elevator = _mapper.Map<Elevator>(request.ElevatorDto.Elevator);
                elevator.ProjectId = request.ElevatorDto.ProjectId;
                
                var createdElevator = await _elevatorService.AddElevatorAsync(elevator);
                return Result<Guid>.Success(createdElevator.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
