using Collins_BackEnd.Domain.Entities;
using Collins_BackEnd.Domain.Common;

namespace Collins_BackEnd.Application.Features.Inventory.Queries.Specifications;

public class ActiveInventoryItemsSpecification : BaseSpecification<InventoryItem>
{
    public ActiveInventoryItemsSpecification() : base(x => !x.IsDisabled)
    {
        AddInclude(x => x.Category);
    }
}
