using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.FreezeContract
{
    public class FreezeContractCommand : IRequest<Result<Unit>>
    {
        public Guid ContractId { get; set; }
        public string? Reason { get; set; }
        public DateTime? FreezeEndDate { get; set; }
    }

    public class FreezeContractCommandHandler : IRequestHandler<FreezeContractCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public FreezeContractCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(FreezeContractCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _maintenanceService.FreezeContractAsync(request.ContractId, request.Reason, request.FreezeEndDate);
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

