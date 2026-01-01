using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class MealBill:AuditableEntity
{
    public long MemberId { get; set; }
    public decimal TotalBazar { get; set; }
    public decimal TotalMemberMeal { get; set; }
    public decimal TotalMeal { get; set; }
    public decimal TotalGuestMeal { get; set; }
    public decimal MealRate { get; set; }
    public decimal MealAmount { get;  set; }
    public decimal HouseBill { get; set; }
    public decimal UtilityBill { get; set; }
    public decimal OtherBill { get; set; }
    public decimal TotalPayable { get; set; }
    public long MealCycleId { get; set; }
    public Member Member { get; set; }
    public MealCycle MealCycle { get; set; }

    public decimal CurrentBill { get; set; }
    public decimal GasBill { get; set; }
    public decimal ServantBill { get; set; }
    public ICollection<PaymentTransaction> PaymentTransaction { get; set; } = new List<PaymentTransaction>();
}
