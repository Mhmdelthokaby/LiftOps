using LiftOps_BackEnd.Application.DTOs.Emergency;
using LiftOps_BackEnd.Application.Interfaces.Emergency;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftOps_BackEnd.Application.Features.Emergency.Queries
{
    public class GetAllEmergencyTicketsQuery : IRequest<Result<IReadOnlyList<EmergencyTicketDto>>>
    {
    }

    public class GetAllEmergencyTicketsQueryHandler : IRequestHandler<GetAllEmergencyTicketsQuery, Result<IReadOnlyList<EmergencyTicketDto>>>
    {
        private readonly IEmergencyService _service;

        public GetAllEmergencyTicketsQueryHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<IReadOnlyList<EmergencyTicketDto>>> Handle(GetAllEmergencyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _service.GetAllTicketsAsync();
            var dtos = tickets.Select(t => new EmergencyTicketDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Project = t.Project,
                Location = t.Location,
                UnitId = t.UnitId,
                GoogleMapsLink = t.GoogleMapsLink,
                Priority = t.Priority,
                Status = t.Status,
                Description = t.Description,
                ReportedBy = t.ReportedBy,
                ReportedAt = t.ReportedAt,
                Contact = t.Contact,
                AssignedTechnicianId = t.AssignedTechnicianId,
                AssignedTechnicianName = t.AssignedTechnician?.Name,
                Notes = t.Notes,
                ResolvedDate = t.ResolvedDate
            }).ToList();

            return Result<IReadOnlyList<EmergencyTicketDto>>.Success(dtos);
        }
    }

    public class GetEmergencyTicketByIdQuery : IRequest<Result<EmergencyTicketDto>>
    {
        public Guid TicketId { get; set; }
    }

    public class GetEmergencyTicketByIdQueryHandler : IRequestHandler<GetEmergencyTicketByIdQuery, Result<EmergencyTicketDto>>
    {
        private readonly IEmergencyService _service;

        public GetEmergencyTicketByIdQueryHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<EmergencyTicketDto>> Handle(GetEmergencyTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _service.GetTicketByIdAsync(request.TicketId);
            if (ticket == null)
                return Result<EmergencyTicketDto>.Failure(new[] { "Emergency ticket not found" });

            var dto = new EmergencyTicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Project = ticket.Project,
                Location = ticket.Location,
                UnitId = ticket.UnitId,
                GoogleMapsLink = ticket.GoogleMapsLink,
                Priority = ticket.Priority,
                Status = ticket.Status,
                Description = ticket.Description,
                ReportedBy = ticket.ReportedBy,
                ReportedAt = ticket.ReportedAt,
                Contact = ticket.Contact,
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedTechnicianName = ticket.AssignedTechnician?.Name,
                Notes = ticket.Notes,
                ResolvedDate = ticket.ResolvedDate
            };

            return Result<EmergencyTicketDto>.Success(dto);
        }
    }

    public class GetOpenEmergencyTicketsQuery : IRequest<Result<IReadOnlyList<EmergencyTicketDto>>>
    {
    }

    public class GetOpenEmergencyTicketsQueryHandler : IRequestHandler<GetOpenEmergencyTicketsQuery, Result<IReadOnlyList<EmergencyTicketDto>>>
    {
        private readonly IEmergencyService _service;

        public GetOpenEmergencyTicketsQueryHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<IReadOnlyList<EmergencyTicketDto>>> Handle(GetOpenEmergencyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _service.GetOpenTicketsAsync();
            var dtos = tickets.Select(t => new EmergencyTicketDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Project = t.Project,
                Location = t.Location,
                UnitId = t.UnitId,
                GoogleMapsLink = t.GoogleMapsLink,
                Priority = t.Priority,
                Status = t.Status,
                Description = t.Description,
                ReportedBy = t.ReportedBy,
                ReportedAt = t.ReportedAt,
                Contact = t.Contact,
                AssignedTechnicianId = t.AssignedTechnicianId,
                AssignedTechnicianName = t.AssignedTechnician?.Name,
                Notes = t.Notes,
                ResolvedDate = t.ResolvedDate
            }).ToList();

            return Result<IReadOnlyList<EmergencyTicketDto>>.Success(dtos);
        }
    }

    public class GetEmergencyTicketsByTechnicianQuery : IRequest<Result<IReadOnlyList<EmergencyTicketDto>>>
    {
        public Guid TechnicianId { get; set; }
    }

    public class GetEmergencyTicketsByTechnicianQueryHandler : IRequestHandler<GetEmergencyTicketsByTechnicianQuery, Result<IReadOnlyList<EmergencyTicketDto>>>
    {
        private readonly IEmergencyService _service;

        public GetEmergencyTicketsByTechnicianQueryHandler(IEmergencyService service)
        {
            _service = service;
        }

        public async Task<Result<IReadOnlyList<EmergencyTicketDto>>> Handle(GetEmergencyTicketsByTechnicianQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _service.GetTicketsByTechnicianAsync(request.TechnicianId);
            var dtos = tickets.Select(t => new EmergencyTicketDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Project = t.Project,
                Location = t.Location,
                UnitId = t.UnitId,
                GoogleMapsLink = t.GoogleMapsLink,
                Priority = t.Priority,
                Status = t.Status,
                Description = t.Description,
                ReportedBy = t.ReportedBy,
                ReportedAt = t.ReportedAt,
                Contact = t.Contact,
                AssignedTechnicianId = t.AssignedTechnicianId,
                AssignedTechnicianName = t.AssignedTechnician?.Name,
                Notes = t.Notes,
                ResolvedDate = t.ResolvedDate
            }).ToList();

            return Result<IReadOnlyList<EmergencyTicketDto>>.Success(dtos);
        }
    }
}

