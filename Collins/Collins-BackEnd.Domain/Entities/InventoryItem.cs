using Collins_BackEnd.Domain.Common;

namespace Collins_BackEnd.Domain.Entities;

public class InventoryItem : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string ItemNumber { get; set; } = string.Empty; // Serial/Part Number
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string AddedByAdminName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
}
