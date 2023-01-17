namespace BrandexBusinessSuite.Inventory.Services.Materials;

using EFCore.BulkExtensions;
using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;

using Data.Enums;
using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Inventory.Data.Models;

public class MaterialsService : IMaterialsService
{
    private readonly InventoryDbContext _db;
    private readonly IMapper _mapper;

    public MaterialsService(InventoryDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task<List<MaterialOutputModel>> GetAll()
        => await _mapper.ProjectTo<MaterialOutputModel>(_db.Materials).ToListAsync();

    public async Task UploadBulk(List<ErpProduct> products, MaterialType materialType, MaterialMeasurement materialMeasurement)
    {
        
        var entities = products.Select(product => new Material()
        {
            Name = product.Name!.BG!.TrimEnd(),
            ErpId = product.Id,
            PartNumber = product.PartNumber!,
            Type = materialType,
            Measurement = materialMeasurement,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();

        await _db.BulkInsertAsync(entities);
    }
}