using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace EntityFrameworkNet5.Data;

public class AuditEntry
{
    public EntityEntry Entry { get; }
    public string Action { get; set; }
    public string TableName { get; set; }
    public Dictionary<string, object> KeyValues { get; set; } = new Dictionary<string, object>();
    public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();
    public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();
    public ICollection<PropertyEntry> TemporaryProperties { get; set; } = new List<PropertyEntry>();

    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public bool HasTemporaryProperties => TemporaryProperties.Any();

    public Audit ToAudit()
    {
        var audit = new Audit
        {
            DateTime = DateTime.Now,
            Action = Action,
            TableName = TableName,
            KeyValues = JsonConvert.SerializeObject(KeyValues),
            OldValues = SerialiseDictionary(OldValues),
            NewValues = SerialiseDictionary(NewValues),
        };

        return audit;
    }

    private string SerialiseDictionary(Dictionary<string, object> values)
    {
        return values == null || values.Count == 0 ? null : JsonConvert.SerializeObject(values);
    }
}
