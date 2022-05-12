namespace BrandexSalesAdapter.ExcelLogic.Services.Products;

using System.Collections.Generic;
using System.Threading.Tasks;
using BrandexSalesAdapter.ExcelLogic.Models.Products;

public interface IProductsService
{
    Task<string> CreateProduct(ProductInputModel productInputModel);

    Task<List<ProductCheckModel>> GetProductsCheck();

    Task<bool> CheckProductByDistributor(string input, string distributor);

    Task<int> ProductIdByDistributor(string input, string distributor);

    Task<ICollection<ProductDistributorCheck>> ProductsIdByDistributorForCheck(string input);

    Task<string> NameById(string input, string distributor);

    Task<IEnumerable<string>> GetProductsNames();

    Task<IEnumerable<int>> GetProductsId();

    Task<IEnumerable<ProductShortOutputModel>> GetProductsIdPrices();
}