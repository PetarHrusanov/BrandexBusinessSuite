namespace BrandexBusinessSuite.SalesAnalysis.Services.Pharmacies;

using AutoMapper;
using EFCore.BulkExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.EntityFrameworkCore;

using Data;
using Models.Pharmacies;
using Models.Sales;
using BrandexBusinessSuite.SalesAnalysis.Data.Models;

public class PharmaciesService : IPharmaciesService
{
    private readonly SalesAnalysisDbContext _db;
    private readonly IMapper _mapper;

    public PharmaciesService(SalesAnalysisDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task UploadBulk(List<PharmacyDbInputModel> pharmacies)
    {
        
        var entities = pharmacies.Select(o => new Pharmacy
        {
            BrandexId = o.BrandexId,
            Name = o.Name,
            PharmacyClass = o.PharmacyClass,
            Active = o.Active,
            CompanyId = o.CompanyId,
            PharmacyChainId = o.PharmacyChainId,
            Address = o.Address,
            CityId = o.CityId,
            PharmnetId = o.PharmnetId,
            PhoenixId = o.PhoenixId,
            SopharmaId = o.SopharmaId,
            StingId = o.StingId,
            RegionId = o.RegionId,
            ErpId = o.ErpId,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();
        
        await _db.BulkInsertAsync(entities);
    }
    
    public async Task<List<PharmacyCheckModel>> GetAllCheck() 
        => await _mapper.ProjectTo<PharmacyCheckModel>(_db.Pharmacies).ToListAsync();

    public async Task<List<PharmacyCheckErpModel>> GetAllCheckErp()
        => await _mapper.ProjectTo<PharmacyCheckErpModel>(_db.Pharmacies).ToListAsync();

    public async Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId)
    {
        var pharmacies = _db.Pharmacies.AsQueryable();
        if (regionId != null) pharmacies = pharmacies.Where(p => p.RegionId == regionId);
        
        return await pharmacies.Select(p => new PharmacyExcelModel 
        { 
            Name = p.Name,
            Address = p.Address,
            PharmacyClass = p.PharmacyClass,
            Region = p.Region.Name,
            Sales = p.Sales
                .Where(d => d.Date >= dateBegin &&  d.Date<=dateEnd)
                .Select(s => new SaleExcelOutputModel 
                { 
                    Name = s.Product.Name, 
                    ProductId = s.ProductId, 
                    Count = s.Count, 
                    ProductPrice = s.Product.Price 
                }).ToList()
        }).ToListAsync();
    }

    public async Task BulkUpdateData(List<PharmacyDbUpdateModel> list)
    {
        var pharmacies = await _db.Pharmacies.ToDictionaryAsync(c => c.Id);
        
        var entities = list.Select(o => new Pharmacy
        {
            BrandexId = pharmacies[o.Id].BrandexId,
            Name = o.Name,
            PharmacyClass = pharmacies[o.Id].PharmacyClass,
            Active = pharmacies[o.Id].Active,
            CompanyId = o.CompanyId,
            PharmacyChainId = o.PharmacyChainId,
            Address = o.Address,
            CityId = pharmacies[o.Id].CityId,
            PharmnetId = o.PharmnetId,
            PhoenixId = o.PhoenixId,
            SopharmaId = o.SopharmaId,
            StingId = o.StingId,
            RegionId = o.RegionId,
            ErpId = pharmacies[o.Id].ErpId,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();
        
        await _db.BulkUpdateAsync(entities);
    }
    
}