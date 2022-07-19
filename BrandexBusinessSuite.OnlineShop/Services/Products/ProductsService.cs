using BrandexBusinessSuite.OnlineShop.Data;
using BrandexBusinessSuite.OnlineShop.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.OnlineShop.Services.Products;

public class ProductsService : IProductsService
{
    private OnlineShopDbContext db;

    public ProductsService(OnlineShopDbContext db)
    {
        this.db = db;
    }

    public async Task<List<Product>> GetCheckModels()
     => await db.Products.ToListAsync();
   
}