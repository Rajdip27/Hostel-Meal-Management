using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealCycle:AuditableEntity
{
    public string Name { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int TotalDays { get; set; }
    public ICollection<MealBill> MealBills { get; set; } = new List<MealBill>();
    public ICollection<PaymentTransaction> PaymentTransaction { get; set; } = new List<PaymentTransaction>();
    public ICollection<NormalPayment> NormalPayment { get; set; } = new List<NormalPayment>();

}
