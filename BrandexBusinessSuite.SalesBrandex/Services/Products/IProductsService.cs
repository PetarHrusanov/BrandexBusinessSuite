using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.Products;

public interface IProductsService
{
    Task<List<BasicCheckErpModel>> GetProductsCheck();
}