using Collins_BackEnd.Application.DTOs;
using Collins_BackEnd.Application.Features.Inventory.Queries.Specifications;
using Collins_BackEnd.Domain.Entities;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;

namespace Collins_BackEnd.Application.Features.Inventory.Queries.GetActiveInventoryItems;

public record GetActiveInventoryItemsQuery() : IRequest<List<InventoryItemDto>>;

public class GetActiveInventoryItemsQueryHandler : IRequestHandler<GetActiveInventoryItemsQuery, List<InventoryItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActiveInventoryItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<InventoryItemDto>> Handle(GetActiveInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ActiveInventoryItemsSpecification();
        var items = await _unitOfWork.Repository<InventoryItem>().ListAsync(spec);
        
        return items.Select(item => new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            ItemNumber = item.ItemNumber,
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name ?? "N/A",
            UnitPrice = item.UnitPrice,
            StockQuantity = item.StockQuantity,
            SupplierName = item.SupplierName,
            AddedByAdminName = item.AddedByAdminName,
            CreatedAt = item.CreatedAt,
            IsDisabled = item.IsDisabled
        }).ToList();
    }
}
