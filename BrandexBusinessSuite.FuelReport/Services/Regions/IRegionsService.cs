using BrandexBusinessSuite.FuelReport.Models.Regions;

namespace BrandexBusinessSuite.FuelReport.Services.Regions;

public interface IRegionsService
{
    Task<RegionOutputModel[]> GetAll();
}