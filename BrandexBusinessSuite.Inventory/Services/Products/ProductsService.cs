namespace BrandexBusinessSuite.Inventory.Services.Products;


using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Inventory.Data.Models;

using Data;

public class ProductsService : IProductsService
{
    private readonly InventoryDbContext _db;
    public ProductsService(InventoryDbContext db) => _db = db;

    public async Task<List<BasicCheckErpModel>> GetProductsCheck() 
        => await _db.Products.Select(p => new BasicCheckErpModel
        { 
            Id = p.Id, 
            Name = p.Name, 
            ErpId = p.ErpId 
        }).ToListAsync();

    public async Task UploadBulk(IEnumerable<ErpProduct> products, int pills)
    {
        var entities = products.Select(product => new Product()
        {
            Name = product.Name!.BG!.TrimEnd(),
            ErpId = product.Id,
            PartNumber = product.PartNumber!,
            Pills = pills,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();

        await _db.BulkInsertAsync(entities);
    }
    
}