using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.Regions;

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
using static Common.ExcelDataConstants.Generic;

public class RegionsService :IRegionsService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public RegionsService(BrandexSalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
        
    public async Task UploadBulk(List<BasicErpInputModel> cities)
    {
        var table = new DataTable();
        table.TableName = Regions;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(ErpId, typeof(string));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var city in cities)
        {
            var row = table.NewRow();
            row[Name] = city.Name.TrimEnd().ToUpper();
            row[ErpId] = city.ErpId;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
        
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = Regions;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
            
    }

    public async Task<List<BasicCheckErpModel>> GetRegionsCheck()
    {
        return await _db.Cities.Select(p => new BasicCheckErpModel()
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId
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