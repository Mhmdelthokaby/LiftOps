using LiftOps_BackEnd.Application.Features.Inventory.Queries.Specifications;
using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Interfaces;
using MediatR;

namespace LiftOps_BackEnd.Application.Features.Inventory.Queries.GetInventoryTotalValue;

public record GetInventoryTotalValueQuery() : IRequest<decimal>;

public class GetInventoryTotalValueQueryHandler : IRequestHandler<GetInventoryTotalValueQuery, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInventoryTotalValueQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(GetInventoryTotalValueQuery request, CancellationToken cancellationToken)
    {
        var spec = new ActiveInventoryItemsSpecification();
        var items = await _unitOfWork.Repository<InventoryItem>().ListAsync(spec);
        
        return items.Sum(x => x.UnitPrice * x.StockQuantity);
    }
}
