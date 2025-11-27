using HostelMealManagement.Core.Entities.BaseEntities;

namespace HostelMealManagement.Core.Entities;

public class BazarExpenses : AuditableEntity
{
    public DateTimeOffset Date { get; set; }
    public string IteamName { get; set; }
    public decimal Amount { get; set; }
}

