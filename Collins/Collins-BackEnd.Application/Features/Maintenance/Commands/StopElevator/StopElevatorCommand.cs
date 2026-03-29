using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.StopElevator
{
    public class StopElevatorCommand : IRequest<Result<Unit>>
    {
        public Guid ElevatorId { get; set; }
        public string? Reason { get; set; }
    }

    public class StopElevatorCommandHandler : IRequestHandler<StopElevatorCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public StopElevatorCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(StopElevatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _maintenanceService.StopElevatorAsync(request.ElevatorId, request.Reason);
                if (!success) return Result<Unit>.Failure("Elevator not found");
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}

