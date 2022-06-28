using BrandexBusinessSuite.MarketingAnalysis.Models.Products;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Products;

using MarketingAnalysis.Models.Products;

public interface IProductsService
{
    
    Task UploadBulk(List<ProductInputModel> medias);
    
    Task<string> CreateProduct(ProductInputModel productInputModel);

    Task<List<ProductCheckModel>> GetCheckModels();

    Task<string> NameById(string input, string distributor);

    Task<IEnumerable<string>> GetProductsNames();

    Task<IEnumerable<int>> GetProductsId();

}