namespace BrandexBusinessSuite.FuelReport.Services.CarModels;

using BrandexBusinessSuite.FuelReport.Models.CarModels;

public interface ICarModelService
{
    Task<CarModelOutputModel[]> GetAll();
    Task Upload(CarModelInputModel carModel);
}