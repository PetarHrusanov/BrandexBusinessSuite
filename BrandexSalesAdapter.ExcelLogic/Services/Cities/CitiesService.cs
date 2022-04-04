using Microsoft.Data.SqlClient;
using NPOI.SS.Formula.Functions;

namespace BrandexSalesAdapter.ExcelLogic.Services.Cities
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Data.Models;
    
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Models.Cities;
    
    using Microsoft.Extensions.Configuration;
    
    using static Common.DataConstants.CitiesColumns;

    public class CitiesService :ICitiesService
    {
        SpravkiDbContext db;
        private readonly IConfiguration _configuration;
        

        public CitiesService(SpravkiDbContext db, IConfiguration configuration)
        {
            this.db = db;
            _configuration = configuration;
        }
        
        public async Task UploadBulk(List<string> cities)
        {
            var table = new DataTable();
            table.TableName = Cities;
            
            table.Columns.Add(Name, typeof(string));
            
            foreach (var city in cities)
            {
                var row = table.NewRow();
                row[Name] = city;
                table.Rows.Add(row);
            }

            string connection = _configuration.GetConnectionString("DefaultConnection");
            
            var con = new SqlConnection(connection);
            
            var objbulk = new SqlBulkCopy(con);  
            
            objbulk.DestinationTableName = Cities;
            
            objbulk.ColumnMappings.Add(Name, Name);

            con.Open();
            await objbulk.WriteToServerAsync(table);  
            con.Close();  
            
        }

        public Task<bool> CheckCityName(string cityName)
        {
            return db.Cities.Where(x => x.Name.ToLower().TrimEnd().Contains(cityName.ToLower().TrimEnd()))
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
            if (city == null) return "";
            var cityModel = new City
            {
                Name = city
            };
            await db.Cities.AddAsync(cityModel);
            await db.SaveChangesAsync();
            return cityModel.Name;

        }


    }
}
