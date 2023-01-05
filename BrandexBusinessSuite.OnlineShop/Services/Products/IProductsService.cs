namespace BrandexBusinessSuite.OnlineShop.Services.Products;

using BrandexBusinessSuite.OnlineShop.Data.Models;

public interface IProductsService
{
    Task<List<Product>> GetCheckModels();
    Task ChangeBatch(Product product, string erpLot);
    
    Task UpdateProduct(Product product);
}