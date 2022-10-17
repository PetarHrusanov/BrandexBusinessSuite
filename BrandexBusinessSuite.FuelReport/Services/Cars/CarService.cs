using BrandexBusinessSuite.FuelReport.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.FuelReport.Services.Cars;

using AutoMapper;
using BrandexBusinessSuite.FuelReport.Data;
using BrandexBusinessSuite.FuelReport.Models.Cars;

public class CarService : ICarService
{
    private readonly FuelReportDbContext _db;
    private readonly IMapper _mapper;

    public CarService(FuelReportDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CarAssignmentModel[]> GetAllRegistrationId() 
        => await _mapper.ProjectTo<CarAssignmentModel>(_db.Cars).ToArrayAsync();
    

    public async Task Upload(CarInputModel car)
    {
        if (!_db.Cars.Any(c => c.Registration == car.Registration) && _db.CarModels.Any(c => c.Id == car.CarModelId))
        {
            var newCar = new Car
            {
                Registration = car.Registration,
                Mileage = car.Mileage,
                Active = car.Active,
                CarModelId = car.CarModelId
            };
            await _db.AddAsync(newCar);
            await _db.SaveChangesAsync();
        }
    }
}