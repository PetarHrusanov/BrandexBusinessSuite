namespace BrandexBusinessSuite.SalesAnalysis.Services.Cities;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.Models.DataModels;

public interface ICitiesService
{
    Task UploadBulk(List<string> cities);
    Task<string> UploadCity(string city);
    Task<List<BasicCheckErpModel>> GetCitiesCheck();

    Task BulkUpdateData(List<BasicCheckErpModel> list);
}