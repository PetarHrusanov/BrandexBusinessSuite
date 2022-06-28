namespace BrandexBusinessSuite.ExcelLogic.Data.Seeding;

using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Options;

using Models;

using BrandexSalesAdapter;
using BrandexSalesAdapter.Services.Data;

using static BrandexSalesAdapter.Common.ExcelDataConstants.Regions;
using static BrandexSalesAdapter.Common.ExcelDataConstants.Ditributors;

public class ApplicationDbContextSeeder : ISeeder
{
    
    private readonly SpravkiDbContext db;
    // private readonly ApplicationSettings applicationSettings;

    public ApplicationDbContextSeeder(
        SpravkiDbContext db,
        IOptions<ApplicationSettings> applicationSettings)
    {
        this.db = db;
        // this.applicationSettings = applicationSettings.Value;
    }

    public void SeedAsync()
    {
        if (db.Regions.Any()) return;
        foreach (var region in GetRegions())
        {
            db.Regions.Add(region);
        }
        
        db.SaveChanges();
        
        if (db.Distributors.Any()) return;
        foreach (var distributor in GetDistributors())
        {
            db.Distributors.Add(distributor);
        }
        
        db.SaveChanges();
    }
    
    private static IEnumerable<Region> GetRegions()
            => new List<Region>
            {
                new() { Name = CentralOfficeSofia },
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
                new() { Name = Vidin },
                new() { Name = Unserviced },
                new() { Name = OnlineShop },
       
            };
    
    private static IEnumerable<Distributor> GetDistributors()
        => new List<Distributor>
        {
            new() { Name = Brandex },
            new() { Name = Phoenix },
            new() { Name = Sopharma },
            new() { Name = Sting },
            new() { Name = Pharmnet },

        };
    

}