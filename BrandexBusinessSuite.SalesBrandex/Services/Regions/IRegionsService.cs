using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.Regions;

using BrandexBusinessSuite.Models;

public interface IRegionsService
{
    Task UploadBulk(List<BasicErpInputModel> cities);
    Task<List<BasicCheckErpModel>> GetRegionsCheck();
}