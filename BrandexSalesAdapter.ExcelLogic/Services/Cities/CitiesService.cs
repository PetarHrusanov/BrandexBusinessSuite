namespace BrandexSalesAdapter.ExcelLogic.Services.Cities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Models.Cities;

    public class CitiesService :ICitiesService
    {
        SpravkiDbContext db;

        public CitiesService(SpravkiDbContext db)
        {
            this.db = db;
        }

        public Task<bool> CheckCityName(string companyName)
        {
            return db.Cities.Where(x => x.Name.ToLower().TrimEnd().Contains(companyName.ToLower().TrimEnd()))
                                    .Select(x => x.Id).AnyAsync();
        }

        public Task<int> IdByName(string companyName)
        {
            return db.Cities.Where(x => x.Name.ToLower().TrimEnd().Contains(companyName.ToLower().TrimEnd()))
                                    .Select(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<CityCheckModel>> GetCitiesCheck()
        {
            return await db.Cities.Select(p => new CityCheckModel()
            {
                Id = p.Id,
                Name = p.Name
            }).ToListAsync();
        }

        public async Task<string> UploadCity(string city)
        {
            if(city!= null)
            {
                var cityModel = new City
                {
                    Name = city
                };
                await this.db.Cities.AddAsync(cityModel);
                await this.db.SaveChangesAsync();
                return cityModel.Name;
            }
            else
            {
                return "";
            }
        }


    }
}
