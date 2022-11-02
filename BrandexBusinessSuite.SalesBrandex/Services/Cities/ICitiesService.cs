namespace BrandexBusinessSuite.SalesBrandex.Services.Cities;

using BrandexBusinessSuite.Models;

public interface ICitiesService
{
    Task UploadBulk(List<string> cities);
    Task<string> UploadCity(string city);
    Task<List<BasicCheckModel>> GetCitiesCheck();
}