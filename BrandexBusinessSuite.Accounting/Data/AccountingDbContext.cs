namespace BrandexBusinessSuite.Accounting.Data;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.Accounting.Data.Models;
using BrandexBusinessSuite.Data.Models.Common;


public class AccountingDbContext : DbContext
{
    public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<MarketingActivityDetails> MarketingActivityDetails { get; set; }

    public override int SaveChanges() => SaveChanges(true);

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInfoRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(true, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInfoRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfoRules()
    {
        var changedEntries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is IAuditInfo &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in changedEntries)
        {
            var entity = (IAuditInfo)entry.Entity;
            if (entry.State == EntityState.Added && entity.CreatedOn == default)
                entity.CreatedOn = DateTime.UtcNow;
            else
                entity.ModifiedOn = DateTime.UtcNow;
        }
    }
}