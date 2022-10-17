namespace BrandexBusinessSuite.FuelReport.Services.CarModels;

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.FuelReport.Models.CarBrands;
using BrandexBusinessSuite.FuelReport.Models.CarModels;
using BrandexBusinessSuite.FuelReport.Data.Models;
using Data;

public class CarModelService : ICarModelService
{
    private readonly FuelReportDbContext _db;
    private readonly IMapper _mapper;

    public CarModelService(FuelReportDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CarModelOutputModel[]> GetAll()
    {
        return await _mapper.ProjectTo<CarModelOutputModel>(_db.CarModels).ToArrayAsync();
    }

    public async Task Upload(CarModelInputModel carModel)
    {
        if (!_db.CarModels.Any(c => c.Name == carModel.Name) && _db.CarBrands.Any(c => c.Id == carModel.CarBrandId))
        {
            var newCarModel = new CarModel
            {
                Name = carModel.Name,
                CarBrandId = carModel.CarBrandId
            };
            await _db.AddAsync(newCarModel);
            await _db.SaveChangesAsync();
        }
    }
}