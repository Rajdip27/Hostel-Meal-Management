using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealBazarItem: AuditableEntity
{
    public long MealBazarId { get; set; }
    public MealBazar MealBazar { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}
