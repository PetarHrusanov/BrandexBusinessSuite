namespace BrandexBusinessSuite.OnlineShop.Services.Products;

using BrandexBusinessSuite.OnlineShop.Data.Models;

public interface IProductsService
{
    Task<List<Product>> GetCheckModels();
}