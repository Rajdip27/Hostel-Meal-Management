using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Application.CommonModel;

public class FilterViewModel :BaseEntity
{
    public string SelectedMember { get; set; }
    public string MemberCodeNo { get; set; }
    public long MealCycleId { get; set; }
}
