namespace BrandexBusinessSuite.SalesAnalysis.Services.Regions;

using System.Threading.Tasks;
using SalesAnalysis.Models.Regions;

public interface IRegionsService
{
    Task<string> UploadRegion(string regionName);

    Task<RegionOutputModel[]> AllRegions();

}