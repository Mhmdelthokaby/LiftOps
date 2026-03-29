using Collins_BackEnd.Application.DTOs;
using Collins_BackEnd.Application.Interfaces;
using Collins_BackEnd.Domain.Entities;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;

namespace Collins_BackEnd.Application.Features.Inventory.Commands.AddInventoryItem;

public record AddInventoryItemCommand(CreateInventoryItemDto Dto) : IRequest<Guid?>;

public class AddInventoryItemCommandHandler : IRequestHandler<AddInventoryItemCommand, Guid?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddInventoryItemCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid?> Handle(AddInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var item = new InventoryItem
        {
            Name = request.Dto.Name,
            ItemNumber = request.Dto.ItemNumber,
            CategoryId = request.Dto.CategoryId,
            UnitPrice = request.Dto.UnitPrice,
            StockQuantity = request.Dto.StockQuantity,
            SupplierName = request.Dto.SupplierName,
            AddedByAdminName = _currentUserService.UserEmail ?? "Unknown",
            IsDisabled = false
        };

        _unitOfWork.Repository<InventoryItem>().Add(item);
        var result = await _unitOfWork.Complete();

        return result > 0 ? item.Id : null;
    }
}
