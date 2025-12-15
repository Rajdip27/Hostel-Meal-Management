using HostelMealManagement.Core.Entities;

namespace HostelMealManagement.Application.ViewModel;

public class MealBazarVm
{
    public long Id { get; set; }

    public DateTimeOffset BazarDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public int TotalDays { get; set; }

    // 🔹 This will store "1,2,3,4"
    public string MealMemberId { get; set; } = string.Empty;

    public decimal BazarAmount { get; set; }
    public string Description { get; set; } = string.Empty;

    public List<MealBazarItemVm> Items { get; set; } = new();

    // 🔹 UI only (NOT stored in DB)
    public List<MemberLookupVm> Members { get; set; } = new();
}
