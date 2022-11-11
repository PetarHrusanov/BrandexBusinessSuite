using System.Collections.Generic;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System.Threading.Tasks;
using SalesAnalysis.Models.Regions;

public interface IRegionsService
{
    Task<string> UploadRegion(string regionName);
    Task<List<BasicCheckErpModel>> AllRegions();
    Task BulkUpdateData(List<BasicCheckErpModel> list);

}