namespace BrandexBusinessSuite.SalesBrandex.Services.Cities;

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using BrandexBusinessSuite.SalesBrandex.Data.Models;
using Data;
using BrandexBusinessSuite.Models;

using static Common.ExcelDataConstants.CitiesColumns;
using static Common.Constants;

public class CitiesService :ICitiesService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public CitiesService(BrandexSalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
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

    public async Task<List<BasicCheckModel>> GetCitiesCheck()
    {
        return await _db.Cities.Select(p => new BasicCheckModel()
        {
            Id = p.Id,
            Name = p.Name
        }).ToListAsync();
    }

    public async Task<string> UploadCity(string city)
    {
        var cityModel = new City
        {
            Name = city
        };
        await _db.Cities.AddAsync(cityModel);
        await _db.SaveChangesAsync();
        return cityModel.Name;
    }
    
}