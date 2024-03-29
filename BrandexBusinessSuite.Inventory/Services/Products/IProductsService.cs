namespace BrandexBusinessSuite.Inventory.Services.Products;

using BrandexBusinessSuite.Models.ErpDocuments;
using BrandexBusinessSuite.Models.DataModels;

public interface IProductsService
{
    Task<List<BasicCheckErpModel>> GetProductsCheck();
    Task UploadBulk(IEnumerable<ErpProduct> products, int pills);
}