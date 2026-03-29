using LiftOps_BackEnd.Application.Interfaces.Installation;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Technicians.Commands
{
    public class DeleteTechnicianCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }

        public DeleteTechnicianCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteTechnicianCommandHandler : IRequestHandler<DeleteTechnicianCommand, Result<Unit>>
    {
        private readonly ITechnicianService _service;

        public DeleteTechnicianCommandHandler(ITechnicianService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(DeleteTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeleteTechnicianAsync(request.Id);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
