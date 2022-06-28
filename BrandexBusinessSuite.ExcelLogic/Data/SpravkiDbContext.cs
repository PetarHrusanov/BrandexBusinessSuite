namespace BrandexBusinessSuite.ExcelLogic.Data;

using System.Linq;
using System;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.ExcelLogic.Data.Models;
using BrandexSalesAdapter.Data.Models.Common;

public class SpravkiDbContext : DbContext
{
     
    public SpravkiDbContext(DbContextOptions<SpravkiDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }

    public DbSet<Company> Companies { get; set; }

    public DbSet<Pharmacy> Pharmacies { get; set; }

    public DbSet<PharmacyChain> PharmacyChains { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Region> Regions { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<Distributor> Distributors { get; set; }

    // General Logic
    public override int SaveChanges() => this.SaveChanges(true);

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ApplyAuditInfoRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        this.SaveChangesAsync(true, cancellationToken);

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        this.ApplyAuditInfoRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyAuditInfoRules()
    {
        var changedEntries = this.ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is IAuditInfo &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in changedEntries)
        {
            var entity = (IAuditInfo)entry.Entity;
            if (entry.State == EntityState.Added && entity.CreatedOn == default)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
            else
            {
                entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}