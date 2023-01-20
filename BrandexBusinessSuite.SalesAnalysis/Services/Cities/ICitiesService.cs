namespace BrandexBusinessSuite.SalesAnalysis.Services.Cities;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

public interface ICitiesService
{
    Task UploadBulk(List<ErpCityCheck> cities);
    Task<List<BasicCheckErpModel>> GetAllCheck();
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}