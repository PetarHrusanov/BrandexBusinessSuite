namespace BrandexBusinessSuite.FuelReport.Controllers;

using Microsoft.AspNetCore.Mvc;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Services;
using Models.CarBrands;
using Models.CarModels;
using Services.CarModels;
using Services.CarBrands;

using BrandexBusinessSuite.FuelReport.Models.Cars;
using BrandexBusinessSuite.FuelReport.Models.Drivers;
using BrandexBusinessSuite.FuelReport.Services.Cars;
using BrandexBusinessSuite.FuelReport.Services.Drivers;
using BrandexBusinessSuite.FuelReport.Models.DriverCar;
using BrandexBusinessSuite.FuelReport.Models.DriverRegion;
using BrandexBusinessSuite.FuelReport.Models.Regions;
using BrandexBusinessSuite.FuelReport.Services.Regions;


public class AdminController : AdministrationController
{
    private readonly ICarBrandService _carBrandService;
    private readonly ICarModelService _carModelService;
    private readonly ICarService _carService;
    private readonly IDriverService _driverService;
    private readonly IRegionsService _regionsService;

    public AdminController(ICarBrandService carBrandService, ICarModelService carModelService, ICarService carService,
        IDriverService driverService, IRegionsService regionsService)
    {
        _carBrandService = carBrandService;
        _carModelService = carModelService;
        _carService = carService;
        _driverService = driverService;
        _regionsService = regionsService;
    }
    
    [HttpGet]
    public async Task<CarBrandOutputModel[]> GetCarBrands()
    {
        return await _carBrandService.GetAll();
    }
    
    [HttpGet]
    public async Task<CarModelOutputModel[]> GetCarModels()
    {
        return await _carModelService.GetAll();
    }
    
    [HttpGet]
    public async Task<CarAssignmentModel[]> GetCars()
    {
        return await _carService.GetAllRegistrationId();
    }
    
    [HttpGet]
    public async Task<RegionOutputModel[]> GetRegions()
    {
        return await _regionsService.GetAll();
    }
    
    [HttpGet]
    public async Task<DriverSelectionModel[]> GetDriversSelection()
    {
        return await _driverService.GetDriversSelection();
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadCarBrand([FromForm]string carBrand)
    {
        await _carBrandService.Upload(carBrand);
        return Result.Success;
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadCarModel([FromForm] CarModelInputModel carModel)
    {
        await _carModelService.Upload(carModel);
        return Result.Success;
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadCar([FromBody] CarInputModel carInput)
    {
        await _carService.Upload(carInput);
        return Result.Success;
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadDriver([FromBody] DriverInputModel driverInput)
    {
        await _driverService.Upload(driverInput);
        return Result.Success;
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadDriverCar([FromBody] DriverCarInputModel driverCarInput)
    {
        await _driverService.UploadDriverCar(driverCarInput);
        return Result.Success;
    }
    
    [HttpPost]
    public async Task<ActionResult> UploadDriverRegion([FromBody] DriverRegionInputModel driverRegionInput)
    {
        await _driverService.UploadDriverRegion(driverRegionInput);
        return Result.Success;
    }

}