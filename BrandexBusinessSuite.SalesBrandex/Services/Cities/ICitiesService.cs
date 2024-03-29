using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.Cities;

using BrandexBusinessSuite.Models;

public interface ICitiesService
{
    Task UploadBulk(List<BasicErpInputModel> cities);
    Task<string> UploadCity(string city);
    Task<List<BasicCheckErpModel>> GetCitiesCheck();
}