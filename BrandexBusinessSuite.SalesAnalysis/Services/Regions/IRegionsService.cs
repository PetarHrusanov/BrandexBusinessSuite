namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.Models.DataModels;

public interface IRegionsService
{
    Task<List<BasicCheckErpModel>> GetAllCheck();
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}