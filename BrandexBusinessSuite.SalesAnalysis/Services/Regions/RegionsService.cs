namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using BrandexBusinessSuite.Models.DataModels;
using Data;
using SalesAnalysis.Data.Models;

public class RegionsService : IRegionsService
{
    private readonly SalesAnalysisDbContext _db;
    public RegionsService(SalesAnalysisDbContext db) => _db = db;
    
    
    public async Task<List<BasicCheckErpModel>> GetAllCheck() 
        =>await _db.Regions.Select(a => new BasicCheckErpModel
        {
            Id = a.Id,
            ErpId = a.ErpId,
            Name = a.Name
        }).ToListAsync();

    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var pharmacyCompany = await _db.Regions.ToDictionaryAsync(c => c.Id);
        var entities = list.Select(o => new Region()
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