using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.SalesBrandex.Data;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.SalesBrandex.Services.Products;

public class ProductsService : IProductsService
{
    private readonly BrandexSalesAnalysisDbContext _db;
    private readonly IConfiguration _configuration;

    public ProductsService(BrandexSalesAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
    public async Task<List<BasicCheckErpModel>> GetProductsCheck()
    {
        return await _db.Products.Select(p => new BasicCheckErpModel()
        {
            Id = p.Id,
            Name = p.Name,
            ErpId = p.ErpId
        }).ToListAsync();
    }
}