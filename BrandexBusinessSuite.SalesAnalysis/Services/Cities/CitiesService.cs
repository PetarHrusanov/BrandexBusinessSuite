namespace BrandexBusinessSuite.SalesAnalysis.Services.Cities;

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Data;
using SalesAnalysis.Data.Models;
using SalesAnalysis.Models.Cities;

using static Common.ExcelDataConstants.CitiesColumns;
using static Common.Constants;

public class CitiesService :ICitiesService
{
    SalesAnalysisDbContext db;
    private readonly IConfiguration _configuration;

    public CitiesService(SalesAnalysisDbContext db, IConfiguration configuration)
    {
        this.db = db;
        _configuration = configuration;
    }
        
    public async Task UploadBulk(List<string> cities)
    {
        var table = new DataTable();
        table.TableName = Cities;
            
        table.Columns.Add(Name, typeof(string));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var city in cities)
        {
            var row = table.NewRow();
            row[Name] = city;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
        
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = Cities;
            
        objbulk.ColumnMappings.Add(Name, Name);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
            
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