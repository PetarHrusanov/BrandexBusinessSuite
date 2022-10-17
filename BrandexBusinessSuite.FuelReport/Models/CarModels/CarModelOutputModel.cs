using BrandexBusinessSuite.Models;

namespace BrandexBusinessSuite.FuelReport.Models.CarModels;

using AutoMapper;
using BrandexBusinessSuite.FuelReport.Data.Models;

public class CarModelOutputModel : IMapFrom<CarModel>
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<CarModel, CarModelOutputModel>();
}