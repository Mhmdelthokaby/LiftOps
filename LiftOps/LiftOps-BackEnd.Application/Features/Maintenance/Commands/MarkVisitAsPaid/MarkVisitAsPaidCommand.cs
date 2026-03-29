using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.MarkVisitAsPaid
{
    public class MarkVisitAsPaidCommand : IRequest<Result<Unit>>
    {
        public Guid VisitId { get; set; }
    }

    public class MarkVisitAsPaidCommandHandler : IRequestHandler<MarkVisitAsPaidCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _maintenanceService;

        public MarkVisitAsPaidCommandHandler(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        public async Task<Result<Unit>> Handle(MarkVisitAsPaidCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _maintenanceService.MarkVisitAsPaidAsync(request.VisitId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}

