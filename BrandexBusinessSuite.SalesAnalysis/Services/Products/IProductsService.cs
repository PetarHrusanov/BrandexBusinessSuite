using BrandexBusinessSuite.SalesAnalysis.Models.Products;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Products;

using System.Collections.Generic;
using System.Threading.Tasks;

using SalesAnalysis.Models.Products;

public interface IProductsService
{
    Task<string> CreateProduct(ProductInputModel productInputModel);

    Task<List<ProductCheckModel>> GetProductsCheck();

    Task<string> NameById(string input, string distributor);

    Task<IEnumerable<string>> GetProductsNames();

    Task<IEnumerable<int>> GetProductsId();

    Task<IEnumerable<ProductShortOutputModel>> GetProductsIdPrices();
}