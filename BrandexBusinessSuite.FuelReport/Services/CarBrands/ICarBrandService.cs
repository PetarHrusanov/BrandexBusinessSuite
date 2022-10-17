namespace BrandexBusinessSuite.FuelReport.Services.CarBrands;

using BrandexBusinessSuite.FuelReport.Models.CarBrands;

public interface ICarBrandService
{
    Task<CarBrandOutputModel[]> GetAll();
    Task Upload(string carBrand);
}