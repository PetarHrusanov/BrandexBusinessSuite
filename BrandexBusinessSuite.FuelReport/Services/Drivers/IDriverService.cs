using BrandexBusinessSuite.FuelReport.Models.DriverCar;
using BrandexBusinessSuite.FuelReport.Models.Drivers;

namespace BrandexBusinessSuite.FuelReport.Services.Drivers;

public interface IDriverService
{
    Task Upload(DriverInputModel driver);
    
    Task UploadDriverCar(DriverCarInputModel driverCar);

    Task<DriverSelectionModel[]> GetDriversSelection();
}