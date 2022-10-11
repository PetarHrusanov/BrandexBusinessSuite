namespace BrandexBusinessSuite.FuelReport.Data.Seeding;

using BrandexBusinessSuite.Services.Data;
using Models;

using static BrandexBusinessSuite.Common.ExcelDataConstants.Regions;

public class ApplicationDbContextSeeder : ISeeder
{
    private readonly FuelReportDbContext db;

    public ApplicationDbContextSeeder(FuelReportDbContext db)
    {
        this.db = db;
    }
    
    public void SeedAsync()
    {
        if (!db.Regions.Any())
        {
            foreach (var region in GetRegions())
            {
                db.Regions.Add(region);
            }
            db.SaveChanges();
        }
        
        if (!db.CarBrands.Any())
        {
            foreach (var carBrand in GetCarBrands())
            {
                db.CarBrands.Add(carBrand);
            }
            db.SaveChanges();
        }
        
    }
    
    private static IEnumerable<Region> GetRegions()
        => new List<Region>
        {
            new() { Name = Ruse },
            new() { Name = Varna },
            new() { Name = Burgas },
            new() { Name = StaraZagora },
            new() { Name = SofiaLiulin },
            new() { Name = SofiaMladost },
            new() { Name = SofiaKrasnoSelo },
            new() { Name = SofiaDrujba },
            new() { Name = Plovdiv },
            new() { Name = Pleven },
        };
    
    private static IEnumerable<CarBrand> GetCarBrands()
        => new List<CarBrand>
        {
            new() { Name = "Renault" },
            new() { Name = "Citroen" },
            new() { Name = "Toyota" },
        };
    
}

