using Collins_BackEnd.Application.DTOs.Installation;
using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Technicians.Commands
{
    public class AssignTechnicianCommand : IRequest<Result<Unit>>
    {
        public AssignTechnicianDto Dto { get; set; } = null!;
        public Guid AssignedByUserId { get; set; }
    }

    public class AssignTechnicianCommandHandler : IRequestHandler<AssignTechnicianCommand, Result<Unit>>
    {
        private readonly ITechnicianService _technicianService;

        public AssignTechnicianCommandHandler(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }

        public async Task<Result<Unit>> Handle(AssignTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var techId in request.Dto.TechnicianIds)
                {
                    await _technicianService.AssignTechnicianToElevatorAsync(techId, request.Dto.ElevatorId, request.AssignedByUserId);
                }
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
