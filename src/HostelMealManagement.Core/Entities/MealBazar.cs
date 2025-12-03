using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealBazar : AuditableEntity
{
    public DateTimeOffset BazarDate { get; set; }
    public decimal BazarAmount { get; set; }
    public string Description { get; set; }=string.Empty;

}

