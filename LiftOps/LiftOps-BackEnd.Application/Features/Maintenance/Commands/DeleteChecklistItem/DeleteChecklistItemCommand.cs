using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.Maintenance.Commands.DeleteChecklistItem;

public record DeleteChecklistItemCommand(Guid Id) : IRequest<bool>;

public class DeleteChecklistItemCommandHandler : IRequestHandler<DeleteChecklistItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChecklistItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var checklistItem = await _unitOfWork.Repository<Domain.Entities.Maintenance.MaintenanceChecklistItem>()
            .GetByIdAsync(request.Id);

        if (checklistItem == null)
        {
            return false;
        }

        // Soft delete by setting IsActive to false
        checklistItem.IsActive = false;

        _unitOfWork.Repository<Domain.Entities.Maintenance.MaintenanceChecklistItem>().Update(checklistItem);
        var result = await _unitOfWork.Complete();

        return result > 0;
    }
}

