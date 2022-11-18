namespace BrandexBusinessSuite.SalesBrandex.Services.Products;

using BrandexBusinessSuite.Models.DataModels;

public interface IProductsService
{
    Task<List<BasicCheckErpModel>> GetProductsCheck();
}