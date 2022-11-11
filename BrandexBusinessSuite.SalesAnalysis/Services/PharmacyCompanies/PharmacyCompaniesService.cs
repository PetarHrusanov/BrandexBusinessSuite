using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyCompanies;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
    
using Data;
using Data.Models;
    
using Models.PharmacyCompanies;
using Microsoft.Data.SqlClient;
    
using static Common.ExcelDataConstants.CompaniesColumns;
using static Common.ExcelDataConstants.Generic;
using static  Common.Constants;

using static Methods.DataMethods;

public class PharmacyCompaniesService : IPharmacyCompaniesService
{
    SalesAnalysisDbContext db;
    private readonly IConfiguration _configuration;

    public PharmacyCompaniesService(SalesAnalysisDbContext db ,IConfiguration configuration)
    {
        this.db = db;
        _configuration = configuration;
    }
        
    public async Task UploadBulk(List<PharmacyCompanyInputModel> pharmacyCompanies)
    {
        var table = new DataTable();
        table.TableName = PharmacyCompanies;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(Owner);
        table.Columns.Add(VAT);
            
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var pharmacyCompany in pharmacyCompanies)
        {
            var row = table.NewRow();
            row[Name] = pharmacyCompany.Name.ToUpper();
            row[Owner] = pharmacyCompany.Owner;
            row[VAT] = pharmacyCompany.VAT;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = PharmacyCompanies;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(Owner, Owner);
        objbulk.ColumnMappings.Add(VAT, VAT);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
            
    }

    public async Task UploadBulkFromErp(List<ErpPharmacyCheck> pharmacyCompanies)
    {
        var table = new DataTable();
        table.TableName = PharmacyCompanies;
            
        table.Columns.Add(Name);
        table.Columns.Add(ErpId);

        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var pharmacyCompany in pharmacyCompanies)
        {
            var row = table.NewRow();
            row[Name] = pharmacyCompany.ParentParty!.PartyName.BG.ToUpper().TrimEnd();
            row[ErpId] = pharmacyCompany.ParentParty!.PartyId;

            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString("DefaultConnection");
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = PharmacyCompanies;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(ErpId, ErpId);

        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
    }

    public async Task<string> UploadCompany(PharmacyCompanyInputModel company)
    {
        if (company.Name == null) return "";
        var companyModel = new Company
        {
            Name = company.Name,
            VAT = company.VAT,
            Owner = company.Owner
        };

        await db.Companies.AddAsync(companyModel);
        await db.SaveChangesAsync();
        return company.Name;
    }
    
    public async Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck()
    {
        return await db.Companies.Select(p => new PharmacyCompanyCheckModel()
        {
            Id = p.Id,
            Name = p.Name,
            VAT = p.VAT
        }).ToListAsync();
    }
    
    public async Task<List<BasicCheckErpModel>> GetPharmacyCompaniesErpCheck()
    {
        return await db.Companies.Select(p => new BasicCheckErpModel()
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId
        }).ToListAsync();
    }
    
    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var dt = ConvertToDataTable(list);
        
        var connection = _configuration.GetConnectionString("DefaultConnection");

        await using var conn = new SqlConnection(connection);
        await using var command = new SqlCommand($"CREATE TABLE #TmpTable(Id smallint NOT NULL,ErpId nvarchar(50) NOT NULL, Name nvarchar(400) NOT NULL)", conn);
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
            command.CommandText = $"UPDATE P SET P.[ErpId]= T.[ErpId] FROM [{PharmacyCompanies}] AS P INNER JOIN #TmpTable AS T ON P.[Id] = T.[Id] ;DROP TABLE #TmpTable;";
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