using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Application.ViewModel;


public class MealBazarVm:BaseEntity
{
   
    public DateTimeOffset BazarDate { get; set; }
    public decimal BazarAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<MealBazarItemVm> Items { get; set; } = new();
}

public class MealBazarItemVm: BaseEntity
{
   
    public string ProductName { get; set; } =string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}