using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealMenu : AuditableEntity
{
   
    public DayOfWeek DayOfWeek { get; set; }
    public string MealName { get; set; } = string.Empty;
    public string MenuItems { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
