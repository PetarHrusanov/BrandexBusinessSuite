namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Data;

using Models.PharmacyChains;
using Microsoft.Data.SqlClient;
    
using Data.Models;
    
using static BrandexBusinessSuite.Common.ExcelDataConstants.PharmacyChainsColumns;
using static  BrandexBusinessSuite.Common.Constants;

public class PharmacyChainsService : IPharmacyChainsService
{
    SalesAnalysisDbContext db;
    private readonly IConfiguration _configuration;

    public PharmacyChainsService(SalesAnalysisDbContext db , IConfiguration configuration)
    {
        this.db = db;
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

    public async Task<string> UploadPharmacyChain(string chainName)
    {
        var chainInput = new PharmacyChain
        {
            Name = chainName
        };

        await db.PharmacyChains.AddAsync(chainInput);
        await db.SaveChangesAsync();
        return chainName;

    }
    
    public async Task<List<PharmacyChainCheckModel>> GetPharmacyChainsCheck()
    {
        return await db.PharmacyChains.Select(p => new PharmacyChainCheckModel()
        {
            Id = p.Id,
            Name = p.Name
        }).ToListAsync();
    }
}