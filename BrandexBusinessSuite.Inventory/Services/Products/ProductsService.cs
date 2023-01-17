using BrandexBusinessSuite.Inventory.Data.Models;
using EFCore.BulkExtensions;

namespace BrandexBusinessSuite.Inventory.Services.Products;

using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

using Data;

using static  Common.Constants;
using static  Common.ExcelDataConstants.Generic;


public class ProductsService : IProductsService
{
    private readonly InventoryDbContext _db;
    private readonly IConfiguration _configuration;

    private const string Pills = "Pills";

    public ProductsService(InventoryDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
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