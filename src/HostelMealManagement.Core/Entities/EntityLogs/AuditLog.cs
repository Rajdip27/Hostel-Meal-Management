using HostelMealManagement.Core.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace HostelMealManagement.Core.Entities.EntityLogs;

public class AuditLog : AuditableEntity
{
    public long UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public DateTimeOffset DateTime { get; set; }
    public string OldValues { get; set; } = "{}";           // default empty JSON object
    public string NewValues { get; set; } = "{}";           // default empty JSON object
    public string AffectedColumns { get; set; } = "[]";     // default empty JSON array
    public string PrimaryKey { get; set; } = "{}";          // default empty JSON object
}

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }

    public long UserId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
    public AuditType AuditType { get; set; } = AuditType.None;
    public List<string> ChangedColumns { get; } = new List<string>();

    public AuditLog ToAuditLog()
    {
        var audit = new AuditLog
        {
            UserId = UserId,
            Type = AuditType.ToString(),
            TableName = TableName,
            DateTime = DateTime.Now,
            PrimaryKey = KeyValues.Count == 0 ? "{}" : JsonConvert.SerializeObject(KeyValues),
            OldValues = OldValues.Count == 0 ? "{}" : JsonConvert.SerializeObject(OldValues),
            NewValues = NewValues.Count == 0 ? "{}" : JsonConvert.SerializeObject(NewValues),
            AffectedColumns = ChangedColumns.Count == 0 ? "[]" : JsonConvert.SerializeObject(ChangedColumns)
        };

        switch (AuditType)
        {
            case AuditType.Create:
                audit.CreatedDate = DateTime.Now;
                audit.CreatedBy = UserId;
                break;

            case AuditType.Update:
                audit.ModifiedDate = DateTime.Now;
                audit.ModifiedBy = UserId;
                break;

            case AuditType.Delete:
                audit.ModifiedDate = DateTime.Now;
                audit.ModifiedBy = UserId;
                audit.IsDelete = true;
                break;

            default:
                break;
        }

        return audit;
    }
}

public enum AuditType
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}
