namespace BrandexSalesAdapter.MarketingAnalysis.Services.Products;

using BrandexSalesAdapter.MarketingAnalysis.Models.Products;

public interface IProductsService
{
    
    Task UploadBulk(List<ProductInputModel> medias);
    
    Task<string> CreateProduct(ProductInputModel productInputModel);

    Task<List<ProductCheckModel>> GetProductsCheck();

    Task<string> NameById(string input, string distributor);

    Task<IEnumerable<string>> GetProductsNames();

    Task<IEnumerable<int>> GetProductsId();

}