using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Application.ViewModel;

public class MealAttendanceVm : BaseEntity
{
    public long MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;

    public DateTimeOffset MealDate { get; set; }

    // Member meal
    public bool IsBreakfast { get; set; }
    public int BreakfastQty { get; set; }

    public bool IsLunch { get; set; }
    public int LunchQty { get; set; }

    public bool IsDinner { get; set; }
    public int DinnerQty { get; set; }

    // Guest meal
    public bool IsGuest { get; set; }

    public bool GuestIsBreakfast { get; set; }
    public int GuestBreakfastQty { get; set; }

    public bool GuestIsLunch { get; set; }
    public int GuestLunchQty { get; set; }

    public bool GuestIsDinner { get; set; }
    public int GuestDinnerQty { get; set; }
}
