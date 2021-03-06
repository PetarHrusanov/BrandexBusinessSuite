namespace BrandexBusinessSuite.SalesAnalysis.Services.Products;

using System.Collections.Generic;
using System.Threading.Tasks;

using SalesAnalysis.Models.Products;

public interface IProductsService
{
    Task<string> CreateProduct(ProductInputModel productInputModel);

    Task<List<ProductCheckModel>> GetProductsCheck();

    Task<IEnumerable<string>> GetProductsNames();

    Task<IEnumerable<ProductShortOutputModel>> GetProductsIdPrices();
}