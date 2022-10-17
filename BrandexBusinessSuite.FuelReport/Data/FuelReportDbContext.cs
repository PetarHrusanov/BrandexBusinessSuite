namespace BrandexBusinessSuite.FuelReport.Data;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.Data.Models.Common;
using Models;
public class FuelReportDbContext: DbContext
{
    public FuelReportDbContext(DbContextOptions<FuelReportDbContext> options)
        : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<CarBrand> CarBrands { get; set; }
    public DbSet<CarModel> CarModels { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    
    public DbSet<DriverCar> DriversCars { get; set; }
    
    public DbSet<DriverRegion> DriverRegions { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<RouteLog> RouteLogs { get; set; }

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
    {
        modelBuilder.Entity<DriverRegion>().HasKey(vf=> new {vf.DriverId, vf.RegionId});
        modelBuilder.Entity<DriverCar>().HasKey(vf=> new {vf.DriverId, vf.CarId});
    }
}