namespace BrandexBusinessSuite.SalesAnalysis.Data.Seeding;

using System.Collections.Generic;
using System.Linq;

using Models;
using BrandexBusinessSuite.Services.Data;

using static Common.ExcelDataConstants.Regions;
using static Common.ExcelDataConstants.Ditributors;

public class ApplicationDbContextSeeder : ISeeder
{
    
    private readonly SalesAnalysisDbContext _db;
    public ApplicationDbContextSeeder(SalesAnalysisDbContext db)
        => _db = db;

    public void SeedAsync()
    {
        if (!_db.Regions.Any())
        {
            foreach (var region in GetRegions())
            {
                _db.Regions.Add(region);
            }
            _db.SaveChanges();
        }

        if (_db.Distributors.Any()) return;
        foreach (var distributor in GetDistributors())
        {
            _db.Distributors.Add(distributor);
        }
        _db.SaveChanges();
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