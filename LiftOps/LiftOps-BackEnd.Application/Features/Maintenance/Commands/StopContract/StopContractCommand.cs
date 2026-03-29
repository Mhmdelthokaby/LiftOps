using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.StopContract
{
    public class StopContractCommand : IRequest<Result<Unit>>
    {
        public Guid ContractId { get; set; }
        public string? Reason { get; set; }
    }

    public class StopContractCommandHandler : IRequestHandler<StopContractCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public StopContractCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(StopContractCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _maintenanceService.StopContractAsync(request.ContractId, request.Reason);
                if (!success) return Result<Unit>.Failure("Contract not found");
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}

