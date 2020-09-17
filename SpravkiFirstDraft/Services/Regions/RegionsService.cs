using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpravkiFirstDraft.Data;
using SpravkiFirstDraft.Data.Models;

namespace SpravkiFirstDraft.Services.Regions
{
    public class RegionsService :IRegionsService
    {
        SpravkiDbContext db;

        public RegionsService(SpravkiDbContext db)
        {
            this.db = db;
        }
        public async Task<bool> CheckRegionByName(string regionName)
        {
           return await this.db.Regions.Where(x => x.Name.ToLower().TrimEnd().Contains(regionName.ToLower().TrimEnd()))
                                    .Select(x => x.Id)
                                    .AnyAsync();
        }

        public async Task<int> IdByName(string regionName)
        {
            return await this.db.Regions.Where(x => x.Name.ToLower().TrimEnd().Contains(regionName.ToLower().TrimEnd()))
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
        }

        public async Task<string> UploadRegion(string regionName)
        {
            if (regionName != null)
            {
                var regionDbModel = new Region
                {
                    Name = regionName
                };

                await this.db.Regions.AddAsync(regionDbModel);
                await this.db.SaveChangesAsync();
                return regionName;
            }
            else
            {
                return "";
            }
        }
    }
}
