namespace BrandexSalesAdapter.ExcelLogic.Services.Cities
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using BrandexSalesAdapter.ExcelLogic.Models.Cities;
    

    public interface ICitiesService
    {
        Task<string> UploadCity(string city);

        Task<bool> CheckCityName(string companyName);

        Task<int> IdByName(string companyName);

        Task<List<CityCheckModel>> GetCitiesCheck();
        
    }
}
