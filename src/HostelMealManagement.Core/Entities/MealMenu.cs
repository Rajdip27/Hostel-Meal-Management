using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealMenu : AuditableEntity
{

    public DateTime Date { get; set; }
    public string BreakfastIteam { get; set; }
    public string LunchIteam { get; set; }
    public string DinnerIteam { get; set; }
    public string UpdateBy { get; set; }
}



