namespace HostelMealManagement.Application.ViewModel;

public class ElectricBillVm
{
    public long Id { get; set; }
    public long MealCycleId { get; set; }
    public string MealCycleName { get; set; } = string.Empty;

    public decimal PreviousUnit { get; set; }
    public decimal CurrentUnit { get; set; }
    public decimal PerUnitRate { get; set; }

    public decimal TotalUnit { get; set; }
    public decimal TotalAmount { get; set; }
}
