namespace BrandexBusinessSuite.FuelReport.Services.Drivers;

using BrandexBusinessSuite.FuelReport.Models.DriverCar;
using BrandexBusinessSuite.FuelReport.Models.DriverRegion;
using BrandexBusinessSuite.FuelReport.Models.Drivers;

public interface IDriverService
{
    Task Upload(DriverInputModel driver);
    
    Task UploadDriverCar(DriverCarInputModel driverCar);
    
    Task UploadDriverRegion(DriverRegionInputModel driverRegionInput);

    Task<DriverSelectionModel[]> GetDriversSelection();

    Task<int> GetDriverId(string userId);
}