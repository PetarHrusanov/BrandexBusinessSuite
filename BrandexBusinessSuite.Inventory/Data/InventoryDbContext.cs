using BrandexBusinessSuite.Data.Models.Common;
using BrandexBusinessSuite.Inventory.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.Inventory.Data;

public class InventoryDbContext: DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Material> Materials { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    public override int SaveChanges() => SaveChanges(true);

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInfoRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(true, cancellationToken);

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
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
            else
            {
                entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.Entity<Recipe>().HasKey(vf=> new {vf.MaterialId, vf.ProductId});
}