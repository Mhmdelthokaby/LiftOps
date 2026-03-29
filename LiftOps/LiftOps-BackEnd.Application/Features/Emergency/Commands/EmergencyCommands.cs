using LiftOps_BackEnd.Application.DTOs.Emergency;
using LiftOps_BackEnd.Application.Interfaces.Emergency;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Emergency.Commands
{
    public class CreateEmergencyTicketCommand : IRequest<Result<Guid>>
    {
        public CreateEmergencyTicketDto Dto { get; set; } = null!;
    }

    public class CreateEmergencyTicketCommandHandler : IRequestHandler<CreateEmergencyTicketCommand, Result<Guid>>
    {
        private readonly IEmergencyService _service;

        public CreateEmergencyTicketCommandHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(CreateEmergencyTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new Domain.Entities.Emergency.EmergencyTicket
            {
                Project = request.Dto.Project,
                Location = request.Dto.Location,
                UnitId = request.Dto.UnitId,
                GoogleMapsLink = request.Dto.GoogleMapsLink,
                Priority = request.Dto.Priority,
                Description = request.Dto.Description,
                ReportedBy = request.Dto.ReportedBy,
                Contact = request.Dto.Contact
            };

            var created = await _service.CreateTicketAsync(ticket);
            return Result<Guid>.Success(created.Id);
        }
    }

    public class UpdateEmergencyTicketCommand : IRequest<Result<Unit>>
    {
        public Guid TicketId { get; set; }
        public UpdateEmergencyTicketDto Dto { get; set; } = null!;
    }

    public class UpdateEmergencyTicketCommandHandler : IRequestHandler<UpdateEmergencyTicketCommand, Result<Unit>>
    {
        private readonly IEmergencyService _service;

        public UpdateEmergencyTicketCommandHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(UpdateEmergencyTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new Domain.Entities.Emergency.EmergencyTicket
            {
                Project = request.Dto.Project,
                Location = request.Dto.Location,
                UnitId = request.Dto.UnitId,
                GoogleMapsLink = request.Dto.GoogleMapsLink,
                Priority = request.Dto.Priority,
                Status = request.Dto.Status,
                Description = request.Dto.Description,
                ReportedBy = request.Dto.ReportedBy,
                Contact = request.Dto.Contact,
                AssignedTechnicianId = request.Dto.AssignedTechnicianId,
                Notes = request.Dto.Notes
            };

            await _service.UpdateTicketAsync(request.TicketId, ticket);
            return Result<Unit>.Success(Unit.Value);
        }
    }

    public class DeleteEmergencyTicketCommand : IRequest<Result<Unit>>
    {
        public Guid TicketId { get; set; }
    }

    public class DeleteEmergencyTicketCommandHandler : IRequestHandler<DeleteEmergencyTicketCommand, Result<Unit>>
    {
        private readonly IEmergencyService _service;

        public DeleteEmergencyTicketCommandHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(DeleteEmergencyTicketCommand request, CancellationToken cancellationToken)
        {
            await _service.DeleteTicketAsync(request.TicketId);
            return Result<Unit>.Success(Unit.Value);
        }
    }

    public class AssignEmergencyTechnicianCommand : IRequest<Result<Unit>>
    {
        public Guid TicketId { get; set; }
        public AssignEmergencyTechnicianDto Dto { get; set; } = null!;
    }

    public class AssignEmergencyTechnicianCommandHandler : IRequestHandler<AssignEmergencyTechnicianCommand, Result<Unit>>
    {
        private readonly IEmergencyService _service;

        public AssignEmergencyTechnicianCommandHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(AssignEmergencyTechnicianCommand request, CancellationToken cancellationToken)
        {
            await _service.AssignTechnicianAsync(request.TicketId, request.Dto.TechnicianId);
            return Result<Unit>.Success(Unit.Value);
        }
    }

    public class ResolveEmergencyTicketCommand : IRequest<Result<Unit>>
    {
        public Guid TicketId { get; set; }
        public string? Notes { get; set; }
    }

    public class ResolveEmergencyTicketCommandHandler : IRequestHandler<ResolveEmergencyTicketCommand, Result<Unit>>
    {
        private readonly IEmergencyService _service;

        public ResolveEmergencyTicketCommandHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(ResolveEmergencyTicketCommand request, CancellationToken cancellationToken)
        {
            await _service.ResolveTicketAsync(request.TicketId, request.Notes);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}

