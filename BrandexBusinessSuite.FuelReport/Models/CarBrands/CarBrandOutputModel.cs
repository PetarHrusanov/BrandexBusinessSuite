using AutoMapper;

namespace BrandexBusinessSuite.FuelReport.Models.CarBrands;

using BrandexBusinessSuite.FuelReport.Data.Models;
using BrandexBusinessSuite.Models;

public class CarBrandOutputModel : IMapFrom<CarBrand>
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<CarBrand, CarBrandOutputModel>();
}