namespace SpravkiFirstDraft.Services.Cities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;

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
