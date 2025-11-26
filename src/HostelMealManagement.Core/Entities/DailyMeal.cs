using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class DailyMeal : AuditableEntity
{
    public int MealId { get; set; }
    public int Id { get; set; }
    public DateTime Mealdate { get; set; }
    public string Breakfast { get; set; }
    public string Lunch { get; set; }
    public string Dinner { get; set; }

}






