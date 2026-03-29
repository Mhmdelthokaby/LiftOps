using LiftOps_BackEnd.Application.DTOs.Faults;
using LiftOps_BackEnd.Application.Interfaces.Faults;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace LiftOps_BackEnd.Application.Features.Faults.Commands
{
    public class CreateFaultTicketCommand : IRequest<Result<string>> // Returns Ticket Number
    {
        public CreateFaultTicketDto Dto { get; set; } = null!;
    }

    public class CreateFaultTicketCommandHandler : IRequestHandler<CreateFaultTicketCommand, Result<string>>
    {
        private readonly IFaultService _service;

        public CreateFaultTicketCommandHandler(IFaultService service)
        {
            _service = service;
        }

        public async Task<Result<string>> Handle(CreateFaultTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new Domain.Entities.Faults.FaultTicket
            {
                CustomerName = request.Dto.CustomerName,
                Phone = request.Dto.Phone,
                ProjectAddress = request.Dto.ProjectAddress,
                ProjectNumber = request.Dto.ProjectNumber,
                GoogleMapsLink = request.Dto.GoogleMapsLink,
                ElevatorType = request.Dto.ElevatorType,
                FaultDescription = request.Dto.FaultDescription,
                Severity = request.Dto.Severity
            };

            var created = await _service.CreateTicketAsync(ticket);
            return Result<string>.Success(created.TicketNumber);
        }
    }

    public class AssignFaultTechnicianCommand : IRequest<Result<Unit>>
    {
        public Guid TicketId { get; set; }
        public AssignFaultTechnicianDto Dto { get; set; } = null!;
    }

    public class AssignFaultTechnicianCommandHandler : IRequestHandler<AssignFaultTechnicianCommand, Result<Unit>>
    {
        private readonly IFaultService _service;

        public AssignFaultTechnicianCommandHandler(IFaultService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(AssignFaultTechnicianCommand request, CancellationToken cancellationToken)
        {
            await _service.AssignTechnicianAsync(request.TicketId, request.Dto.TechnicianId);
            return Result<Unit>.Success(Unit.Value);
        }
    }
    
     public class ResolveFaultCommand : IRequest<Result<Unit>>
    {
        public Guid TicketId { get; set; }
        public ResolveFaultDto Dto { get; set; } = null!;
    }

    public class ResolveFaultCommandHandler : IRequestHandler<ResolveFaultCommand, Result<Unit>>
    {
        private readonly IFaultService _service;

        public ResolveFaultCommandHandler(IFaultService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(ResolveFaultCommand request, CancellationToken cancellationToken)
        {
            await _service.ResolveTicketAsync(request.TicketId, request.Dto.Notes);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
