using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class ElectricBill : AuditableEntity
{
    public long MealCycleId { get; set; }

    public decimal PreviousUnit { get; set; }
    public decimal CurrentUnit { get; set; }
    public decimal PerUnitRate { get; set; }

    public MealCycle MealCycle { get; set; } = null!;
}
