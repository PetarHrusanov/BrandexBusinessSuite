namespace BrandexBusinessSuite.FuelReport.Models.Cars;

using AutoMapper;
using BrandexBusinessSuite.FuelReport.Data.Models;
using BrandexBusinessSuite.Models;
public class CarAssignmentModel : IMapFrom<Car>
{
    public int Id { get; set; }
    
    public string Registration { get; set; }
    
    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<Car, CarAssignmentModel>();
}