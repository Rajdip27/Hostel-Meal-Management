using HostelMealManagement.Core.Entities;

namespace HostelMealManagement.Application.ViewModel;

public class MealBazarVm
{
    public long Id { get; set; }

    public DateTimeOffset BazarDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int TotalDays { get; set; }

    public string MemberIds { get; set; } =string.Empty;
    public decimal BazarAmount { get; set; }
    public string Description { get; set; } = string.Empty;

    public string MemberName { get; set; } = string.Empty;

    public List<MealBazarItemVm> Items { get; set; } = new();
   
}
