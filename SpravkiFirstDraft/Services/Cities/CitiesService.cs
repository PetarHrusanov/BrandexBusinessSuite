namespace SpravkiFirstDraft.Services.Cities
{
    using System;
    using System.Threading.Tasks;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;

    public class CitiesService :ICitiesService
    {
        SpravkiDbContext db;

        public CitiesService(SpravkiDbContext db)
        {
            this.db = db;
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
