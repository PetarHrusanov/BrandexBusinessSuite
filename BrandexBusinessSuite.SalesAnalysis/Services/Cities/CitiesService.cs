using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BrandexBusinessSuite.Common;
using BrandexBusinessSuite.Methods;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.SalesAnalysis.Data;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Cities;

using static ExcelDataConstants.CitiesColumns;
using static ExcelDataConstants.Generic;
using static Constants;

using static DataMethods;

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

    public async Task UploadBulkFromErp(List<ErpCityCheck> cities)
    {
        var table = new DataTable();
        table.TableName = Cities;
            
        table.Columns.Add(Name);
        table.Columns.Add(ErpId);
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var city in cities)
        {
            var row = table.NewRow();
            row[Name] = city!.City!.Value!.TrimEnd().ToUpper();
            row[ErpId] = city!.City!.ValueId!;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
        
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = Cities;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();
    }

    public async Task<List<BasicCheckErpModel>> GetCitiesCheck()
    {
        return await db.Cities.Select(p => new BasicCheckErpModel
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId
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

    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var dt = ConvertToDataTable(list);
        
        var connection = _configuration.GetConnectionString("DefaultConnection");

        await using var conn = new SqlConnection(connection);
        await using var command = new SqlCommand("CREATE TABLE #TmpTable(Id smallint NOT NULL,ErpId nvarchar(50) NOT NULL, Name nvarchar(50) NOT NULL)", conn);
        try
        {
            conn.Open();
            command.ExecuteNonQuery();

            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.BulkCopyTimeout = 6600;
                bulkCopy.DestinationTableName = "#TmpTable";
                await bulkCopy.WriteToServerAsync(dt);
                bulkCopy.Close();
            }

            command.CommandTimeout = 3000;
            command.CommandText = "UPDATE P SET P.[ErpId]= T.[ErpId] FROM [Cities] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id] ;DROP TABLE #TmpTable;";
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Handle exception properly
        }
        finally
        {
            conn.Close();
        }
    }

}