using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class GlobalBill : AuditableEntity
{
    public int BillId { get; set; }
    public string Month { get; set; }
    public int HouseBill { get; set; }
    public int UtilityBill { get; set; }
    public int OtherBill { get; set; }
    
}