using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class GuestMeals : AuditableEntity
{
    public DateTimeOffset Date { get; set; }
    public string GuestCount { get; set; }
}