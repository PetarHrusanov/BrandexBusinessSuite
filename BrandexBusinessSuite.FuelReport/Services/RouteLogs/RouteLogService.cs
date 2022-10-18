using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.FuelReport.Services.RouteLogs;

using BrandexBusinessSuite.FuelReport.Data;
using BrandexBusinessSuite.FuelReport.Models.RouteLogs;

public class RouteLogService : IRouteLogService
{
    private readonly FuelReportDbContext _db;
 
    public RouteLogService(FuelReportDbContext db)
    {
        _db = db;
    }
    
    public async Task Upload(RouteInputModel routeInput, int driverId)
    {
        var car =  await _db.DriversCars.Where(d => d.DriverId == driverId && d.Active == true).Select(c => c.Car)
            .FirstOrDefaultAsync();
        var routeLog = new RouteLog()
        {
            Km = routeInput.Kilometers,
            MileageStart = car.Mileage,
            MileageEnd = car.Mileage + routeInput.Kilometers,
            Date = routeInput.Date,
            DriverCarCarId = car.Id,
            DriverCarDriverId = driverId

        };

        car.Mileage += routeInput.Kilometers;
        await _db.AddAsync(routeLog);
        await _db.SaveChangesAsync();
    }

    public async Task<RouteOutputModel> GetLatestRouteByDriver(int driverId)
    {
        return (await _db.RouteLogs
            .Where(d => d.DriverCarDriverId == driverId)
            .OrderByDescending(o => o.Date)
            .Select(r => new RouteOutputModel()
            {
                Name = r.DriverCar.Driver.Name,
                Registration = r.DriverCar.Car.Registration,
                Km = r.Km,
                MileageStart = r.MileageStart,
                MileageEnd = r.MileageEnd,
                Date = r.Date

            }).FirstOrDefaultAsync())!;
    }
}