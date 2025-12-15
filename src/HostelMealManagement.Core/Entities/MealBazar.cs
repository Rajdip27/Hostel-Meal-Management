using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealBazar : AuditableEntity
{
    public DateTimeOffset BazarDate { get; set; }
    // Domain-specific dates
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int TotalDays {  get; set; }
    // Comma-separated Member IDs: "1,2,3"
    public string MealMemberId { get; set; } = string.Empty;
    public decimal BazarAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<MealBazarItem> Items { get; set; } = new();
}
