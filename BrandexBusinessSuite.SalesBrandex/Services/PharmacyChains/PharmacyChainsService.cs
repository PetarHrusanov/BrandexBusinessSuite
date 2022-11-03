using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.PharmacyChains;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using BrandexBusinessSuite.Models;
using Data.Models;
using Data;
    
using static Common.ExcelDataConstants.PharmacyChainsColumns;
using static Common.Constants;
using static Common.ExcelDataConstants.Generic;

public class PharmacyChainsService : IPharmacyChainsService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public PharmacyChainsService(BrandexSalesAnalysisDbContext db , IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
        
    public async Task UploadBulk(List<BasicErpInputModel> pharmacyChains)
    {
        var table = new DataTable();
        table.TableName = PharmacyChains;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(ErpId, typeof(string));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));

        foreach (var pharmacyChain in pharmacyChains)
        {
            var row = table.NewRow();
            row[Name] = pharmacyChain.Name;
            row[ErpId] = pharmacyChain.ErpId;
            
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
}