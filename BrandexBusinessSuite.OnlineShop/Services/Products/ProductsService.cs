using BrandexBusinessSuite.OnlineShop.Data;
using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.OnlineShop.Services.Products;

public class ProductsService : IProductsService
{
    private readonly OnlineShopDbContext _db;
    public ProductsService(OnlineShopDbContext db) 
        =>_db = db;

    public async Task<List<Product>> GetCheckModels()
     => await _db.Products.ToListAsync();

    public async Task UpdateProduct(Product product)
    {
        var productDb = await _db.Products.Where(p => p.Id == product.Id).FirstOrDefaultAsync();
        productDb!.ErpPriceCode = product.ErpPriceCode;
        productDb!.ErpPriceNoVat = product.ErpPriceNoVat;
        await _db.SaveChangesAsync();
    }
}