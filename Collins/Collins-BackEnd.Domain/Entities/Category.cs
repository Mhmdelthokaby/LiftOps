using Collins_BackEnd.Domain.Common;

namespace Collins_BackEnd.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation property
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
