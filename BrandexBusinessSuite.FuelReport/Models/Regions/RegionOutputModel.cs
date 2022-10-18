namespace BrandexBusinessSuite.FuelReport.Models.Regions;

using AutoMapper;
using BrandexBusinessSuite.FuelReport.Data.Models;
using BrandexBusinessSuite.Models;

public class RegionOutputModel :IMapFrom<Region>
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Region, RegionOutputModel>();
}