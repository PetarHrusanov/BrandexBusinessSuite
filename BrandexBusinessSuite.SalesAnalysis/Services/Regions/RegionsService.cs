namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Data;
using SalesAnalysis.Data.Models;
using SalesAnalysis.Models.Regions;

public class RegionsService : IRegionsService
{
    private readonly SpravkiDbContext db;

    public RegionsService(SpravkiDbContext db)
    {
        this.db = db;
    }

    public async Task<bool> CheckRegionByName(string regionName)
    {
        return await db.Regions.Where(x => x.Name.ToLower().TrimEnd().Contains(regionName.ToLower().TrimEnd()))
            .Select(x => x.Id)
            .AnyAsync();
    }

    public async Task<int> IdByName(string regionName)
    {
        return await db.Regions.Where(x => x.Name.ToLower().TrimEnd().Contains(regionName.ToLower().TrimEnd()))
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
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

    public async Task<List<SelectListItem>> RegionsForSelect()
    {
        return await db.Regions.Select(a =>
            new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToListAsync();
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