namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using Data;
using Data.Models;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;


public class PharmacyChainsService : IPharmacyChainsService
{
    private readonly SalesAnalysisDbContext _db;
    public PharmacyChainsService(SalesAnalysisDbContext db) => _db = db;

    public async Task UploadBulk(List<ErpPharmacyCheck> pharmacyChains)
    {
        var entities = pharmacyChains.Select(o => new PharmacyChain
        {
            Name = o.PharmacyChain!.Value!.TrimEnd().ToUpper(),
            ErpId = o.PharmacyChain!.ValueId!,
            CreatedOn = DateTime.Now,
            IsDeleted = false,
        }).ToList();
        
        await _db.BulkInsertAsync(entities);
    }

    public async Task<List<BasicCheckErpModel>> GetAllCheck() 
        => await _db.PharmacyChains.Select(p => new BasicCheckErpModel
        { 
            Id = p.Id, 
            Name = p.Name, 
            ErpId = p.ErpId 
        }).ToListAsync();
    
    
    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var pharmacyChains = await _db.PharmacyChains.ToDictionaryAsync(c => c.Id);
        var entities = list.Select(o => new PharmacyChain
        {
            Id = o.Id,
            Name = o.Name,
            ErpId = o.ErpId,
            ModifiedOn = DateTime.Now,
            CreatedOn = pharmacyChains[o.Id].CreatedOn,
            IsDeleted = pharmacyChains[o.Id].IsDeleted
        }).ToList();
        
        await _db.BulkUpdateAsync(entities);
    }
}