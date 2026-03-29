using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Technicians.Commands
{
    public class UnassignTechnicianCommand : IRequest<Result<Unit>>
    {
        public Guid TechnicianId { get; set; }
        public Guid ElevatorId { get; set; }

        public UnassignTechnicianCommand(Guid technicianId, Guid elevatorId)
        {
            TechnicianId = technicianId;
            ElevatorId = elevatorId;
        }
    }

    public class UnassignTechnicianCommandHandler : IRequestHandler<UnassignTechnicianCommand, Result<Unit>>
    {
        private readonly ITechnicianService _service;

        public UnassignTechnicianCommandHandler(ITechnicianService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(UnassignTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.UnassignTechnicianFromElevatorAsync(request.TechnicianId, request.ElevatorId);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
