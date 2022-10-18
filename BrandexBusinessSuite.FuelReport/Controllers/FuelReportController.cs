using BrandexBusinessSuite.FuelReport.Services.RouteLogs;

namespace BrandexBusinessSuite.FuelReport.Controllers;

using Microsoft.AspNetCore.Mvc;

using BrandexBusinessSuite.Services.Identity;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.FuelReport.Models.RouteLogs;
using BrandexBusinessSuite.FuelReport.Services.Drivers;
using BrandexBusinessSuite.Services;


public class FuelReportController : ApiController
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IDriverService _driverService;
    private readonly ICurrentUserService _currentUser;
    private readonly IRouteLogService _routeLogService;
    public FuelReportController(IWebHostEnvironment hostEnvironment, IDriverService driverService,
        ICurrentUserService currentUser, IRouteLogService routeLogService)
    {
        _hostEnvironment = hostEnvironment;
        _driverService = driverService;
        _currentUser = currentUser;
        _routeLogService = routeLogService;
    }
    
    [HttpGet]
    public async Task<RouteOutputModel> GetLatestActivity()
    {
        var userId = _currentUser.UserId;
        var driverId = _driverService.GetDriverId(userId);

        return await _routeLogService.GetLatestRouteByDriver(await driverId);
    }

    [HttpPost]
    public async Task<ActionResult> Upload([FromBody] RouteInputModel routeInput)
    {
        var userId = _currentUser.UserId;
        var driverId = _driverService.GetDriverId(userId);

        await _routeLogService.Upload(routeInput, await driverId);
        
        return Result.Success;
    }
    
}