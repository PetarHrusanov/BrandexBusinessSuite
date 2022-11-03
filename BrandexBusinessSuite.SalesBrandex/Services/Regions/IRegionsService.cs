using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.Regions;

using BrandexBusinessSuite.Models;

public interface IRegionsService
{
    Task UploadBulk(List<BasicErpInputModel> cities);
    Task<string> UploadCity(string city);
    Task<List<BasicCheckErpModel>> GetRegionsCheck();
}