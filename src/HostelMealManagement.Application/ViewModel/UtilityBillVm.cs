using System;
using HostelMealManagement.Core.Entities;

namespace HostelMealManagement.Application.ViewModel;

public class UtilityBillVm
{
    public long Id { get; set; }

    public UtilityType UtilityType { get; set; }

    // ===== BILLING PERIOD =====
    public int BillYear { get; set; }
    public int BillMonth { get; set; }
    public DateTimeOffset BillDate { get; set; }

    // ===== ELECTRIC =====
    public decimal? CurrentUnit { get; set; }
    public decimal? PerUnitRate { get; set; }

    // ===== OTHER BILLS =====
    public decimal WaterAmount { get; set; }
    public decimal GasAmount { get; set; }
    public decimal ServantAmount { get; set; }

    // ===== COMMON =====
    public decimal TotalUnit { get; set; }
    public decimal TotalAmount { get; set; }
}
