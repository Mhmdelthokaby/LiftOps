using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.FreezeElevator
{
    public class FreezeElevatorCommand : IRequest<Result<Unit>>
    {
        public Guid ElevatorId { get; set; }
        public string? Reason { get; set; }
        public DateTime? FreezeEndDate { get; set; }
    }

    public class FreezeElevatorCommandHandler : IRequestHandler<FreezeElevatorCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public FreezeElevatorCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(FreezeElevatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _maintenanceService.FreezeElevatorAsync(request.ElevatorId, request.Reason, request.FreezeEndDate);
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

