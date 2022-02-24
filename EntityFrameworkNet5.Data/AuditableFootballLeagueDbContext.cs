using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkNet5.Data;
public abstract class AuditableFootballLeagueDbContext : DbContext
{

    public DbSet<Audit> Audits { get; set; }

    public async Task<int> SaveChangesAsync(string username)
    {
        var auditEntries = await OnBeforeSaveChangesAsync(username);
        var saveResult = await base.SaveChangesAsync();

        if (auditEntries != null && auditEntries.Count() > 0)
            await OnAfterSaveChangesAsync(auditEntries);

        return saveResult;
    }

    private async Task<IEnumerable<AuditEntry>> OnBeforeSaveChangesAsync(string username)
    {
        var auditEntries = new List<AuditEntry>();
        var entries = ChangeTracker.Entries().Where(e => e.State != EntityState.Detached && e.State != EntityState.Unchanged);

        foreach (var entry in entries)
        {
            var auditableObject = (BaseDomainObject)entry.Entity;
            auditableObject.ModifiedDate = DateTime.Now;
            auditableObject.ModifiedBy = username;

            if (entry.State == EntityState.Added)
            {
                auditableObject.CreatedDate = DateTime.Now;
                auditableObject.CreatedBy = username;
            }

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName(),
                Action = entry.State.ToString(),
            };

            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        foreach (var pendingAuditEntry in auditEntries.Where(q => !q.HasTemporaryProperties))
            await Audits.AddAsync(pendingAuditEntry.ToAudit());

        return auditEntries.Where(q => q.HasTemporaryProperties);
    }

    private async Task<int> OnAfterSaveChangesAsync(IEnumerable<AuditEntry> auditEntries)
    {
        foreach (var auditEntry in auditEntries)
        {
            foreach (var property in auditEntry.TemporaryProperties)
            {
                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                else
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
            }

            await Audits.AddAsync(auditEntry.ToAudit());
        }

        return await SaveChangesAsync();
    }

}
