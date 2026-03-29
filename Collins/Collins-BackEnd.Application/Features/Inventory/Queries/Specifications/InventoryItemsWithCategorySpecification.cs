using Collins_BackEnd.Domain.Entities;
using Collins_BackEnd.Domain.Common;

namespace Collins_BackEnd.Application.Features.Inventory.Queries.Specifications;

public class InventoryItemsWithCategorySpecification : BaseSpecification<InventoryItem>
{
    public InventoryItemsWithCategorySpecification()
    {
        AddInclude(x => x.Category);
    }
}
