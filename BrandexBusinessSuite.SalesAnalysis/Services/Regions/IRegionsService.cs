using BrandexBusinessSuite.SalesAnalysis.Models.Regions;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System.Collections.Generic;
using System.Threading.Tasks;
using SalesAnalysis.Models.Regions;
using Microsoft.AspNetCore.Mvc.Rendering;

public interface IRegionsService
{
    Task<string> UploadRegion(string regionName);

    Task<bool> CheckRegionByName(string regionName);

    Task<int> IdByName(string regionName);

    Task<List<SelectListItem>> RegionsForSelect();

    Task<RegionOutputModel[]> AllRegions();

}