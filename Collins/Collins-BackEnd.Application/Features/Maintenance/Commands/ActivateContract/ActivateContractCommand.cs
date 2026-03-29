using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.ActivateContract
{
    public class ActivateContractCommand : IRequest<Result<Unit>>
    {
        public Guid ContractId { get; set; }
    }

    public class ActivateContractCommandHandler : IRequestHandler<ActivateContractCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public ActivateContractCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(ActivateContractCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _maintenanceService.ActivateContractAsync(request.ContractId);
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

