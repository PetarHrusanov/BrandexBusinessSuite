using BrandexBusinessSuite.FuelReport.Models.Cars;

namespace BrandexBusinessSuite.FuelReport.Services.Cars;

public interface ICarService
{
    Task<CarAssignmentModel[]> GetAllRegistrationId();
    Task Upload(CarInputModel car);
}