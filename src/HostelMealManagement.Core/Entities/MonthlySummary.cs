using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MonthlySummary : AuditableEntity
{
    public string Month { get; set; }
    public int TotalMeal { get; set; }
    public int GuestMeal { get; set; }
    public int PerMealRate { get; set; }
    public int MealCost { get; set; }
    public int HouseBill { get; set; }
    public int UtilityBill { get; set; }
    public int OtherBill { get; set; }
    public int TotalBill { get; set; }

}






