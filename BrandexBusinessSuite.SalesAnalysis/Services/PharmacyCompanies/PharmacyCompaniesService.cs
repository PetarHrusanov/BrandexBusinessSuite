namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyCompanies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using Data;
using Data.Models;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;


public class PharmacyCompaniesService : IPharmacyCompaniesService
{
    private readonly SalesAnalysisDbContext _db;
    public PharmacyCompaniesService(SalesAnalysisDbContext db) => _db = db;

    public async Task UploadBulk(List<ErpPharmacyCheck> pharmacyCompanies)
    {
        var entities = pharmacyCompanies.Select(o => new Company
        {
            Name = o.ParentParty!.PartyName.BG.ToUpper().TrimEnd(),
            ErpId = o.ParentParty!.PartyId,
            CreatedOn = DateTime.Now,
            IsDeleted = false,
        }).ToList();
        
        await _db.BulkInsertAsync(entities);
    }

    public async Task<List<BasicCheckErpModel>> GetAllCheck()
    => await _db.Companies.Select(p => new BasicCheckErpModel
    { 
        Id = p.Id, 
        Name = p.Name, 
        ErpId = p.ErpId 
    }).ToListAsync();

    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var pharmacyCompany = await _db.Companies.ToDictionaryAsync(c => c.Id);
        var entities = list.Select(o => new Company
        {
            Id = o.Id,
            Name = o.Name,
            ErpId = o.ErpId,
            ModifiedOn = DateTime.Now,
            CreatedOn = pharmacyCompany[o.Id].CreatedOn,
            IsDeleted = pharmacyCompany[o.Id].IsDeleted
        }).ToList();
        
        await _db.BulkUpdateAsync(entities);
    }
}