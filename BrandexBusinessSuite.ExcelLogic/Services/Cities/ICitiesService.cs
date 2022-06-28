namespace BrandexBusinessSuite.ExcelLogic.Services.Cities;

using System.Threading.Tasks;
using System.Collections.Generic;
    
using BrandexBusinessSuite.ExcelLogic.Models.Cities;
    

public interface ICitiesService
{
        
    Task UploadBulk(List<string> cities);
        
    Task<string> UploadCity(string city);

    Task<bool> CheckCityName(string cityName);

    Task<int> IdByName(string companyName);

    Task<List<CityCheckModel>> GetCitiesCheck();
        
}