using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealBazar : AuditableEntity
{
    public DateTimeOffset BazarDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int TotalDays { get; set; }

    public string MealMemberId { get; set; } = string.Empty;

    public decimal BazarAmount { get; set; }
    public string Description { get; set; }=string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int TotalDayCount { get; set; }
    //1,2,3
    //1
    //2
    //3
    public string MealMemberId{ get; set; }=string.Empty;//1,2,3,4,56
    public List<MealBazarItem> Items { get; set; } = new();

}

