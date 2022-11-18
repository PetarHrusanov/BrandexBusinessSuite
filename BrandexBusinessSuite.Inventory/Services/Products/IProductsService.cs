using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.Inventory.Services.Products;

using BrandexBusinessSuite.Models.DataModels;

public interface IProductsService
{
    Task<List<BasicCheckErpModel>> GetProductsCheck();
    Task UploadBulk(List<ErpProduct> products);
}