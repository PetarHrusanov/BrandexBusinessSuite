namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Data;
using SalesAnalysis.Data.Models;
using SalesAnalysis.Models.Regions;

public class RegionsService : IRegionsService
{
    private readonly SalesAnalysisDbContext db;

    public RegionsService(SalesAnalysisDbContext db)
    {
        this.db = db;
    }

    public async Task<string> UploadRegion(string regionName)
    {
        if (regionName == null) return "";
        var regionDbModel = new Region
        {
            Name = regionName
        };

        await db.Regions.AddAsync(regionDbModel);
        await db.SaveChangesAsync();
        return regionName;

    }

    public async Task<RegionOutputModel[]> AllRegions()
    {
        var regionArray = await db.Regions.Select(a =>
            new RegionOutputModel
            {
                Id = a.Id,
                Name = a.Name
            }).ToArrayAsync();
        return regionArray;
    }
}