namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Data;
using BrandexBusinessSuite.MarketingAnalysis.Models.Companies;
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models.DataModels;

using static Common.ExcelDataConstants.CompaniesColumns;
using static Common.Constants;
using static Common.ExcelDataConstants.Generic;

public class CompaniesService : ICompaniesService
{
    
    private readonly MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    
    public CompaniesService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task UploadBulk(List<CompaniesInputModel> companies)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString(DefaultConnection));
        con.Open();
        using (var bulkCopy = new SqlBulkCopy(con))
        {
            bulkCopy.DestinationTableName = PharmacyCompanies;
            var dataTable = new DataTable();
            dataTable.Columns.Add(Name, typeof(string));
            dataTable.Columns.Add(ErpId, typeof(string));
            dataTable.Columns.Add(CreatedOn, typeof(DateTime));
            dataTable.Columns.Add(IsDeleted, typeof(bool));
            foreach (var company in companies)
            {
                var row = dataTable.NewRow();
                row[Name] = company.Name;
                row[ErpId] = company.ErpId;
                row[CreatedOn] = DateTime.Now;
                row[IsDeleted] = false;
                dataTable.Rows.Add(row);
            }
            await bulkCopy.WriteToServerAsync(dataTable);
        }
        con.Close();
    }

    public async Task Upload(BasicErpInputModel inputModel)
    {
        await _db.Companies.AddAsync(new Company { 
            Name = inputModel.Name!.ToUpper().TrimEnd(),
            ErpId = inputModel.ErpId!
        });
        await _db.SaveChangesAsync();
    }

    public async Task<List<BasicCheckModel>> GetCheckModels() 
        => await _db.Companies.Select(p => new BasicCheckModel 
        { 
            Id = p.Id, 
            Name = p.Name 
        }).ToListAsync();

    public async Task<BasicCheckErpModel> Details(int id)
        => await _db.Companies.Where(c => c.Id == id).Select(p => new BasicCheckErpModel
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId
        }).FirstOrDefaultAsync() ?? throw new InvalidOperationException();
   

    public async Task<BasicCheckErpModel> Edit(BasicCheckErpModel inputModel)
    {
        var company = new Company
        {
            Id = inputModel.Id,
            Name = inputModel.Name!,
            ErpId = inputModel.ErpId!
        };
        _db.Companies.Update(company);
        await _db.SaveChangesAsync();
        return inputModel;
    }

    public async Task Delete(int id)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(m => m.Id == id);
        _db.Remove(company);
        await _db.SaveChangesAsync();
    }
}