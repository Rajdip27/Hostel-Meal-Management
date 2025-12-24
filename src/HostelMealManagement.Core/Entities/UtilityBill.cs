using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class UtilityBill : AuditableEntity
{
    public int Month { get; set; }              // 1 = January, 12 = December
    public DateTimeOffset Date { get; set; }

    public decimal CurrentBill { get; set; }    // Electricity / Current bill amount
    public decimal GasBill { get; set; }
    public decimal ServantBill { get; set; }
}
