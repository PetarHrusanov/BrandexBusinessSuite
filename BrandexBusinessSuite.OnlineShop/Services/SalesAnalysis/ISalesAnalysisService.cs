using BrandexBusinessSuite.OnlineShop.Data.Models;
using BrandexBusinessSuite.OnlineShop.Models;

namespace BrandexBusinessSuite.OnlineShop.Services.SalesAnalysis;

public interface ISalesAnalysisService
{
    Task UploadBulk(List<SalesOnlineAnalysisInput> salesAnalysis);
}