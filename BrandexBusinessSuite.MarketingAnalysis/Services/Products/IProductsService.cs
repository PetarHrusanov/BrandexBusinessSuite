using BrandexBusinessSuite.MarketingAnalysis.Models.Products;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Products;

using MarketingAnalysis.Models.Products;

public interface IProductsService
{
    Task UploadBulk(List<ProductInputModel> medias);
    Task Upload(ProductInputModel inputModel);
    Task<List<ProductCheckModel>> GetCheckModels();
    
}