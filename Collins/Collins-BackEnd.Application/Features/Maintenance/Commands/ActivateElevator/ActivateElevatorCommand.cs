using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.ActivateElevator
{
    public class ActivateElevatorCommand : IRequest<Result<Unit>>
    {
        public Guid ElevatorId { get; set; }
    }

    public class ActivateElevatorCommandHandler : IRequestHandler<ActivateElevatorCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public ActivateElevatorCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(ActivateElevatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _maintenanceService.ActivateElevatorAsync(request.ElevatorId);
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

