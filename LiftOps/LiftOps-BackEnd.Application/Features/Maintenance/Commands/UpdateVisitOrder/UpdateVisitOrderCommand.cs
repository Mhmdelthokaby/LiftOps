using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.UpdateVisitOrder
{
    public class UpdateVisitOrderCommand : IRequest<Result<Unit>>
    {
        public UpdateVisitOrderDto Dto { get; set; } = null!;
    }

    public class UpdateVisitOrderCommandHandler : IRequestHandler<UpdateVisitOrderCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public UpdateVisitOrderCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(UpdateVisitOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.UpdateVisitOrderAsync(request.Dto.Date, request.Dto.VisitIds);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
