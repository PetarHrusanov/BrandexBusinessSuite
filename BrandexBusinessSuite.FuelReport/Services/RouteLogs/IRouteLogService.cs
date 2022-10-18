namespace BrandexBusinessSuite.FuelReport.Services.RouteLogs;

using BrandexBusinessSuite.FuelReport.Models.RouteLogs;

public interface IRouteLogService
{
    Task Upload(RouteInputModel routeInput, int driverId);
    
    Task<RouteOutputModel> GetLatestRouteByDriver(int driverId);
}