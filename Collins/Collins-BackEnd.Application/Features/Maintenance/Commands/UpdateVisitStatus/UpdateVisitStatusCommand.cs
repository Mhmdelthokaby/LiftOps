using Collins_BackEnd.Application.DTOs.Maintenance;
using Collins_BackEnd.Application.Interfaces.Maintenance;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Maintenance.Commands.UpdateVisitStatus
{
    public class UpdateVisitStatusCommand : IRequest<Result<Unit>>
    {
        public UpdateVisitStatusDto Dto { get; set; } = null!;
    }

    public class UpdateVisitStatusCommandHandler : IRequestHandler<UpdateVisitStatusCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public UpdateVisitStatusCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(UpdateVisitStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var visit = await _service.GetVisitByIdAsync(request.Dto.VisitId);
                if (visit == null)
                {
                    return Result<Unit>.Failure("Visit not found.");
                }

                // Parse status
                if (!Enum.TryParse<Domain.Entities.Maintenance.VisitStatus>(request.Dto.Status, out var status))
                {
                    return Result<Unit>.Failure($"Invalid status: {request.Dto.Status}");
                }

                // Update status
                visit.Status = status;
                
                // If marking as Done, set completed date
                if (status == Domain.Entities.Maintenance.VisitStatus.Done && !visit.CompletedDate.HasValue)
                {
                    visit.CompletedDate = DateTime.UtcNow;
                }

                // Update visit (assuming there's an update method or we use repository directly)
                // For now, we'll need to add this to the service interface
                await _service.UpdateVisitAsync(visit);
                
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
