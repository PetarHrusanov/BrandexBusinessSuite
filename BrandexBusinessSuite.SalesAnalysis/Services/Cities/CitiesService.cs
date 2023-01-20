namespace BrandexBusinessSuite.SalesAnalysis.Services.Cities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EFCore.BulkExtensions;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;
using Data;

public class CitiesService :ICitiesService
{
    private readonly SalesAnalysisDbContext _db;
    public CitiesService(SalesAnalysisDbContext db) => _db = db;

    public async Task UploadBulk(List<ErpCityCheck> cities)
    {
        var entities = cities.Select(o => new City
        {
            Name = o!.City!.Value!.TrimEnd().ToUpper(),
            ErpId = o!.City!.ValueId!,
            CreatedOn = DateTime.Now,
            IsDeleted = false,
        }).ToList();
        
        await _db.BulkInsertAsync(entities);
    }

    public async Task<List<BasicCheckErpModel>> GetAllCheck() 
        => await _db.Cities.Select(p => new BasicCheckErpModel 
        { 
            Id = p.Id, 
            Name = p.Name, 
            ErpId = p.ErpId 
        }).ToListAsync();

    public async Task BulkUpdateData(List<BasicCheckErpModel> list)
    {
        var cities = await _db.Cities.ToDictionaryAsync(c => c.Id);
        var entities = list.Select(o => new City
        {
            Id = o.Id,
            Name = o.Name,
            ErpId = o.ErpId,
            ModifiedOn = DateTime.Now,
            CreatedOn = cities[o.Id].CreatedOn,
            IsDeleted = cities[o.Id].IsDeleted
        }).ToList();
        
        await _db.BulkUpdateAsync(entities);
    }
}