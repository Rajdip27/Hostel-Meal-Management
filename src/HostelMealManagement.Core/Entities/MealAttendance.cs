using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealAttendance : AuditableEntity
{
    public long MemberId { get; set; }
    public Member Member { get; set; }
    public DateTimeOffset MealDate { get; set; }
    public bool IsBreakfast { get; set; }
    public bool IsLunch { get; set; }
    public bool IsDinner { get; set; }
    //Guest
    public bool IsGuest { get; set; }
    public bool GuestIsBreakfast { get; set; }
    public int GuestBreakfastQty { get; set; }

    public bool GuestIsLunch { get; set; }
    public int GuestLunchQty { get; set; }

    public bool GuestIsDinner { get; set; }
    public int GuestDinnerQty { get; set; }
}
