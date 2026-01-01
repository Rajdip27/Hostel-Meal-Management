using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class PaymentTransaction:AuditableEntity
{
    public string TransactionId { get; set; }
    public string ValidationId { get; set; }
    public decimal Amount { get; set; }
    public decimal StoreAmount { get; set; }
    public string Currency { get; set; }
    public string CardType { get; set; }
    public string CardIssuer { get; set; }
    public string BankTransactionId { get; set; }
    public string Status { get; set; }
    public int RiskLevel { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime ValidatedOn { get; set; }
    public long MemberId { get; set; }
    public long MealCycleId { get; set; }
    public long MealBillId { get; set; }
    public Member Member { get; set; }
    public MealCycle MealCycle { get; set; }
    public MealBill MealBill { get; set; }
}
