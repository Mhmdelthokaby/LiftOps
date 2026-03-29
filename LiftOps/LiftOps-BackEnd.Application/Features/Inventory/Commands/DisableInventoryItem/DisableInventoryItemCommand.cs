using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.Inventory.Commands.DisableInventoryItem;

public record DisableInventoryItemCommand(Guid Id, bool Disable) : IRequest<bool>;

public class DisableInventoryItemCommandHandler : IRequestHandler<DisableInventoryItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DisableInventoryItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DisableInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.Id);
        if (item == null) return false;

        item.IsDisabled = request.Disable;

        _unitOfWork.Repository<InventoryItem>().Update(item);
        var result = await _unitOfWork.Complete();

        return result > 0;
    }
}
