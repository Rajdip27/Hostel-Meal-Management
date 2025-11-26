using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class BazarExpenses : AuditableEntity
{
    public int BazarId { get; set; }
    public DateTimeOffset date { get; set; }
    public string Iteam { get; set; }
    public int Amount { get; set; }
    public string Addedby { get; set; }

    public string Createdby { get; set; }

}

