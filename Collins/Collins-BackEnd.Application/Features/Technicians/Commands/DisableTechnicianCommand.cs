using Collins_BackEnd.Application.Interfaces.Installation;
using Collins_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Collins_BackEnd.Application.Features.Technicians.Commands
{
    public class DisableTechnicianCommand : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
        public bool Disable { get; set; }

        public DisableTechnicianCommand(Guid id, bool disable)
        {
            Id = id;
            Disable = disable;
        }
    }

    public class DisableTechnicianCommandHandler : IRequestHandler<DisableTechnicianCommand, Result<Unit>>
    {
        private readonly ITechnicianService _service;

        public DisableTechnicianCommandHandler(ITechnicianService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(DisableTechnicianCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DisableTechnicianAsync(request.Id, request.Disable);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
