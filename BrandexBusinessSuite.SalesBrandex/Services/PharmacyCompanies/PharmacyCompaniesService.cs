namespace BrandexBusinessSuite.SalesBrandex.Services.PharmacyCompanies;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using AutoMapper;
using BrandexBusinessSuite.SalesBrandex.Models.Pharmacies;
    
using Data;
using Data.Models;
    
using Models.PharmacyCompanies;
using Microsoft.Data.SqlClient;
    
using static Common.ExcelDataConstants.CompaniesColumns;
using static  Common.Constants;
using static Common.ExcelDataConstants.Generic;

public class PharmacyCompaniesService : IPharmacyCompaniesService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PharmacyCompaniesService(BrandexSalesAnalysisDbContext db ,IConfiguration configuration, IMapper mapper)
    {
        _db = db;
        _configuration = configuration;
        _mapper = mapper;
    }
        
    public async Task UploadBulk(List<PharmacyCompanyInputModel> pharmacyCompanies)
    {
        var table = new DataTable();
        table.TableName = PharmacyCompanies;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(ErpId);

        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var pharmacyCompany in pharmacyCompanies)
        {
            var row = table.NewRow();
            row[Name] = pharmacyCompany.Name.ToUpper();
            row[ErpId] = pharmacyCompany.ErpId;

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
            ErpId = company.ErpId,
        };

        await _db.Companies.AddAsync(companyModel);
        await _db.SaveChangesAsync();
        return company.Name;
    }
    
    public async Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck()
        => await _mapper.ProjectTo<PharmacyCompanyCheckModel>(_db.Companies).ToListAsync();
}