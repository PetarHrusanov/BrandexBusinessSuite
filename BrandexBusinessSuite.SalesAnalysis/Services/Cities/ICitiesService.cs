using BrandexBusinessSuite.SalesAnalysis.Models.Cities;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Cities;

using System.Threading.Tasks;
using System.Collections.Generic;
    
using SalesAnalysis.Models.Cities;
    

public interface ICitiesService
{
        
    Task UploadBulk(List<string> cities);
        
    Task<string> UploadCity(string city);

    Task<bool> CheckCityName(string cityName);

    Task<int> IdByName(string companyName);

    Task<List<CityCheckModel>> GetCitiesCheck();
        
}