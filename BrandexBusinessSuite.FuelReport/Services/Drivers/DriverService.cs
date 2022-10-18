namespace BrandexBusinessSuite.FuelReport.Services.Drivers;

using Microsoft.EntityFrameworkCore;

using AutoMapper;

using BrandexBusinessSuite.FuelReport.Data.Models;
using BrandexBusinessSuite.FuelReport.Models.DriverCar;
using BrandexBusinessSuite.FuelReport.Models.DriverRegion;
using BrandexBusinessSuite.FuelReport.Data;
using BrandexBusinessSuite.FuelReport.Models.Drivers;

public class DriverService :IDriverService
{
    private readonly FuelReportDbContext _db;
    private readonly IMapper _mapper;

    public DriverService(FuelReportDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task Upload(DriverInputModel driver)
    {
        var newDriver = new Driver
        {
            Name = driver.Name,
            LastName = driver.LastName,
            UserId = driver.UserId,
            Active = driver.Active,
        };
        await _db.AddAsync(newDriver);
        await _db.SaveChangesAsync();
    }

    public async Task UploadDriverCar(DriverCarInputModel driverCar)
    {
        if (_db.Drivers.Any(d => d.Id == driverCar.DriverId) && _db.Cars.Any(d => d.Id == driverCar.CarId))
        {
            var newDriver = new DriverCar()
            {
                DriverId = driverCar.DriverId,
                CarId = driverCar.CarId,
                Active = driverCar.Active,
            };
            await _db.AddAsync(newDriver);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UploadDriverRegion(DriverRegionInputModel driverRegionInput)
    {
        if (_db.Drivers.Any(d => d.Id == driverRegionInput.DriverId) && _db.Regions.Any(d => d.Id == driverRegionInput.RegionId))
        {
            var newDriver = new DriverRegion()
            {
                DriverId = driverRegionInput.DriverId,
                RegionId = driverRegionInput.RegionId,
                Active = driverRegionInput.Active,
            };
            await _db.AddAsync(newDriver);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<DriverSelectionModel[]> GetDriversSelection()
    {
        return await _db.Drivers.Select(s => new DriverSelectionModel()
        {
            Id = s.Id,
            CompleteName = s.Name + ' ' + s.LastName,
        }).ToArrayAsync();
    }

    public async Task<int> GetDriverId(string userId)
    {
        return await _db.Drivers.Where(d => d.UserId == userId).Select(d => d.Id).FirstOrDefaultAsync();
    }
}