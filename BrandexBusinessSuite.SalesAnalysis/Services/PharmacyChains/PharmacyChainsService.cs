using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

using Data;
using Data.Models;
    
using static BrandexBusinessSuite.Common.ExcelDataConstants.PharmacyChainsColumns;
using static Common.ExcelDataConstants.Generic;
using static  BrandexBusinessSuite.Common.Constants;

using static Methods.DataMethods;


public class PharmacyChainsService : IPharmacyChainsService
{
    private readonly SalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public PharmacyChainsService(SalesAnalysisDbContext db , IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
        
    public async Task UploadBulk(List<string> pharmacyChains)
    {
        var table = new DataTable();
        table.TableName = PharmacyChains;
            
        table.Columns.Add(Name, typeof(string));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var pharmacyChain in pharmacyChains)
        {
            var row = table.NewRow();
            row[Name] = pharmacyChain;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        string connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = PharmacyChains;
            
        objbulk.ColumnMappings.Add(Name, Name);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
            
    }

    public async Task UploadBulkFromErp(List<ErpPharmacyCheck> pharmacyChains)
    {
        var table = new DataTable();
        table.TableName = PharmacyChains;
            
        table.Columns.Add(Name);
        table.Columns.Add(ErpId);
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var pharmacyChain in pharmacyChains)
        {
            var row = table.NewRow();
            row[Name] = pharmacyChain.PharmacyChain!.Value!.TrimEnd().ToUpper();
            row[ErpId] = pharmacyChain.PharmacyChain!.ValueId!;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        string connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = PharmacyChains;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
    }

    public async Task<string> UploadPharmacyChain(string chainName)
    {
        var chainInput = new PharmacyChain
        {
            Name = chainName
        };

        await _db.PharmacyChains.AddAsync(chainInput);
        await _db.SaveChangesAsync();
        return chainName;

    }
    
    public async Task<List<BasicCheckErpModel>> GetPharmacyChainsCheck()
    {
        return await _db.PharmacyChains.Select(p => new BasicCheckErpModel()
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId
        }).ToListAsync();
    }
    
    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        
        var dt = new DataTable(PharmacyChains);
        dt = ConvertToDataTable(list);
        
        var connection = _configuration.GetConnectionString("DefaultConnection");

        await using var conn = new SqlConnection(connection);
        await using var command = new SqlCommand($"CREATE TABLE #TmpTable(Id smallint NOT NULL,ErpId nvarchar(50) NOT NULL, Name nvarchar(50) NOT NULL)", conn);
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
            command.CommandText = $"UPDATE P SET P.[ErpId]= T.[ErpId] FROM [{PharmacyChains}] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id] ;DROP TABLE #TmpTable;";
            command.ExecuteNonQuery();
        }
        catch (Exception)
        {
            // Handle exception properly
        }
        finally
        {
            conn.Close();
        }
    }
}