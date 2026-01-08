using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class TransactionHisotry : AuditableEntity
{
    // Transaction Date
    public DateTimeOffset TransactionDate { get; set; }

    // FK → Member
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = null!;

    // FK → Payment Bill
    public Guid TransactionId { get; set; }
    public PaymentTransaction Transaction { get; set; } = null!;

    // FK → Bill Cycle
    public Guid MealCycleId { get; set; }
    public MealCycle MealCycle { get; set; } = null!;

    // Total Bill Amount
    public decimal Amount { get; set; }

    // Optional but useful
    public string Remarks { get; set; } = string.Empty;
}
