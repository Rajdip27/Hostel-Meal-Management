using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class NormalPayment : AuditableEntity
{
    // Payment date
    public DateTimeOffset PaymentDate { get; set; }

    // Member Id (from Member table)
    public long MemberId { get; set; }

    public long MealCycleId { get; set; }

    // Payment amount (comes from Meal Bill)
    public decimal PaymentAmount { get; set; }

    // Optional notes or remarks
    public string Remarks { get; set; } = string.Empty;

    public Member Member{get; set;}

    public MealCycle MealCycle { get; set; }
    }
