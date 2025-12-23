using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class UtilityBill : AuditableEntity
{
    // Electric / Water / Gas / Servant
    public UtilityType UtilityType { get; set; }

    // Billing period
    public int BillYear { get; set; }      // e.g. 2025
    public int BillMonth { get; set; }     // 1 = January, 12 = December

    // Actual bill issue/payment date
    public DateTimeOffset BillDate { get; set; }

    // Only for Electric Bill
    public decimal? CurrentUnit { get; set; }
    public decimal? PerUnitRate { get; set; }

    // Common for all utility bills
    public decimal TotalUnit { get; set; }
    public decimal TotalAmount { get; set; }
}

