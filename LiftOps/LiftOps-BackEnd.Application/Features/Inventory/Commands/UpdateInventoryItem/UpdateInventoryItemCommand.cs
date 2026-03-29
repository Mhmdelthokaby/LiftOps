using LiftOps_BackEnd.Application.DTOs;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.Inventory.Commands.UpdateInventoryItem;

public record UpdateInventoryItemCommand(Guid Id, UpdateInventoryItemDto Dto) : IRequest<bool>;

public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInventoryItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.Id);
        if (item == null) return false;

        item.Name = request.Dto.Name;
        item.ItemNumber = request.Dto.ItemNumber;
        item.CategoryId = request.Dto.CategoryId;
        item.UnitPrice = request.Dto.UnitPrice;
        item.StockQuantity = request.Dto.StockQuantity;
        item.SupplierName = request.Dto.SupplierName;

        _unitOfWork.Repository<InventoryItem>().Update(item);
        var result = await _unitOfWork.Complete();

        return result > 0;
    }
}
