namespace BrandexBusinessSuite.SalesBrandex.Services.Products;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.Models.DataModels;
using Data;

public class ProductsService : IProductsService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    public ProductsService(BrandexSalesAnalysisDbContext db) => _db = db;
    
    public async Task<List<BasicCheckErpModel>> GetProductsCheck() 
        => await _db.Products.Select(p => new BasicCheckErpModel
        { 
            Id = p.Id, 
            Name = p.Name, 
            ErpId = p.ErpId 
        }).ToListAsync();
}