using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Application.Features.Inventory.Queries.Specifications;

public class InventoryItemsWithCategorySpecification : BaseSpecification<InventoryItem>
{
    public InventoryItemsWithCategorySpecification()
    {
        AddInclude(x => x.Category);
    }
}
