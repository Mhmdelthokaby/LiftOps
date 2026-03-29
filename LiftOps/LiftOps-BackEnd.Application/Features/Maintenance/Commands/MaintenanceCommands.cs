using LiftOps_BackEnd.Application.DTOs.Maintenance;
using LiftOps_BackEnd.Application.Interfaces.Maintenance;
using LiftOps_BackEnd.Application.Common;
using MediatR;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands
{
    public class CreateMaintenanceContractCommand : IRequest<Result<Guid>>
    {
        public CreateMaintenanceContractDto Dto { get; set; } = null!;
    }

    public class CreateMaintenanceContractCommandHandler : IRequestHandler<CreateMaintenanceContractCommand, Result<Guid>>
    {
        private readonly IMaintenanceService _service;
        private readonly IMapper _mapper;

        public CreateMaintenanceContractCommandHandler(IMaintenanceService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateMaintenanceContractCommand request, CancellationToken cancellationToken)
        {
            // Map Dto -> Entity
            // Need mapping profile. Assuming explicit for now or add via profile later.
            var contract = new Domain.Entities.Maintenance.MaintenanceContract
            {
                CustomerId = request.Dto.CustomerId,
                StartDate = request.Dto.StartDate,
                EndDate = request.Dto.EndDate,
                PricePerMonth = request.Dto.PricePerMonth,
                FreeMonths = request.Dto.FreeMonths,
                TechnicianId = request.Dto.TechnicianId,
                Status = Domain.Entities.Maintenance.MaintenanceContractStatus.Active
            };

            var result = await _service.CreateContractAsync(contract);
            return Result<Guid>.Success(result.Id);
        }
    }

    public class ScheduleVisitCommand : IRequest<Result<Guid>>
    {
        public ScheduleVisitDto Dto { get; set; } = null!;
    }

    public class ScheduleVisitCommandHandler : IRequestHandler<ScheduleVisitCommand, Result<Guid>>
    {
        private readonly IMaintenanceService _service;

        public ScheduleVisitCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(ScheduleVisitCommand request, CancellationToken cancellationToken)
        {
            var visit = await _service.ScheduleVisitAsync(request.Dto.ElevatorId, request.Dto.VisitDate, request.Dto.TechnicianId);
            return Result<Guid>.Success(visit.Id);
        }
    }

     public class CompleteVisitCommand : IRequest<Result<Unit>>
    {
        public CompleteVisitDto Dto { get; set; } = null!;
    }

    public class CompleteVisitCommandHandler : IRequestHandler<CompleteVisitCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public CompleteVisitCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(CompleteVisitCommand request, CancellationToken cancellationToken)
        {
            // Map parts
            var parts = request.Dto.PartsUsed.Select(p => (p.ItemId, p.Quantity)).ToList();
            
            // Map checklist items
            List<(Guid ChecklistItemId, bool IsCompleted, string? Notes, int? Count, decimal? Percentage)>? checklistItems = null;
            if (request.Dto.ChecklistItems != null && request.Dto.ChecklistItems.Any())
            {
                checklistItems = request.Dto.ChecklistItems
                    .Select(c => (c.ChecklistItemId, c.IsCompleted, c.Notes, c.Count, c.Percentage))
                    .ToList();
            }
            
            await _service.CompleteVisitAsync(request.Dto.VisitId, request.Dto.Notes, parts, checklistItems, request.Dto.PaymentNotes);
            return Result<Unit>.Success(Unit.Value);
        }
    }

    public class UpdateMaintenanceContractCommand : IRequest<Result<Unit>>
    {
        public Guid ContractId { get; set; }
        public UpdateMaintenanceContractDto Dto { get; set; } = null!;
    }

    public class UpdateMaintenanceContractCommandHandler : IRequestHandler<UpdateMaintenanceContractCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public UpdateMaintenanceContractCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(UpdateMaintenanceContractCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.UpdateContractAsync(request.ContractId, request.Dto);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }

    public class UpdateMaintenanceElevatorCommand : IRequest<Result<Unit>>
    {
        public Guid ElevatorId { get; set; }
        public UpdateMaintenanceElevatorDto Dto { get; set; } = null!;
    }

    public class UpdateMaintenanceElevatorCommandHandler : IRequestHandler<UpdateMaintenanceElevatorCommand, Result<Unit>>
    {
        private readonly IMaintenanceService _service;

        public UpdateMaintenanceElevatorCommandHandler(IMaintenanceService service)
        {
            _service = service;
        }

        public async Task<Result<Unit>> Handle(UpdateMaintenanceElevatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _service.UpdateElevatorAsync(request.ElevatorId, request.Dto);
                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}
