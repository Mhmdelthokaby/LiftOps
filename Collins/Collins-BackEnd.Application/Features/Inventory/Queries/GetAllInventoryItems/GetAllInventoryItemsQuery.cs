using Collins_BackEnd.Application.DTOs;
using Collins_BackEnd.Domain.Entities;
using Collins_BackEnd.Domain.Interfaces;
using MediatR;
using Collins_BackEnd.Application.Features.Inventory.Queries.Specifications;

namespace Collins_BackEnd.Application.Features.Inventory.Queries.GetAllInventoryItems;

public record GetAllInventoryItemsQuery() : IRequest<List<InventoryItemDto>>;

public class GetAllInventoryItemsQueryHandler : IRequestHandler<GetAllInventoryItemsQuery, List<InventoryItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllInventoryItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<InventoryItemDto>> Handle(GetAllInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var spec = new InventoryItemsWithCategorySpecification();
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
