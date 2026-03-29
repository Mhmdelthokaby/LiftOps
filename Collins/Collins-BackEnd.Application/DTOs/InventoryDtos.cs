namespace Collins_BackEnd.Application.DTOs;

public class InventoryItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ItemNumber { get; set; } = string.Empty; // Serial/Part Number
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string AddedByAdminName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDisabled { get; set; }
}

public class CreateInventoryItemDto
{
    public string Name { get; set; } = string.Empty;
    public string ItemNumber { get; set; } = string.Empty; // Serial/Part Number
    public Guid CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}

public class UpdateInventoryItemDto
{
    public string Name { get; set; } = string.Empty;
    public string ItemNumber { get; set; } = string.Empty; // Serial/Part Number
    public Guid CategoryId { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}
