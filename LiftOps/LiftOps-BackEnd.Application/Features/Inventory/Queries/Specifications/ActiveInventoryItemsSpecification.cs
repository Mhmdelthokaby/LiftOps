using LiftOps_BackEnd.Domain.Entities;
using LiftOps_BackEnd.Domain.Common;

namespace LiftOps_BackEnd.Application.Features.Inventory.Queries.Specifications;

public class ActiveInventoryItemsSpecification : BaseSpecification<InventoryItem>
{
    public ActiveInventoryItemsSpecification() : base(x => !x.IsDisabled)
    {
        AddInclude(x => x.Category);
    }
}
