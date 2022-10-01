namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Data;
using BrandexBusinessSuite.MarketingAnalysis.Models.Companies;

using static Common.ExcelDataConstants.CompaniesColumns;
using static Common.Constants;

public class CompaniesService : ICompaniesService
{
    
    private MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    
    public CompaniesService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
    public async Task UploadBulk(List<CompaniesInputModel> companies)
    {
        var table = new DataTable();
        table.TableName = PharmacyCompanies;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(ErpId, typeof(string));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var company in companies)
        {
            var row = table.NewRow();
            row[Name] = company.Name;
            row[ErpId] = company.ErpId;
            
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

    public async Task<List<CompaniesCheckModel>> GetCheckModels()
    {
        return await _db.Companies.Select(p => new CompaniesCheckModel()
        {
            Id = p.Id,
            Name = p.Name
        }).ToListAsync();
    }
}