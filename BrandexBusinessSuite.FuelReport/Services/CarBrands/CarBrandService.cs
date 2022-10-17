using AutoMapper;
using BrandexBusinessSuite.FuelReport.Models.CarBrands;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.FuelReport.Services.CarBrands;

using Data;
using BrandexBusinessSuite.FuelReport.Data.Models;

public class CarBrandService : ICarBrandService
{
    private readonly FuelReportDbContext _db;
    private readonly IMapper _mapper;

    public CarBrandService(FuelReportDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<CarBrandOutputModel[]> GetAll()
    {
        return await _mapper.ProjectTo<CarBrandOutputModel>(_db.CarBrands).ToArrayAsync();
    }

    public async Task Upload(string carBrand)
    {
        var newCarBrand = new CarBrand
        {
            Name = carBrand
        };
        if (!_db.CarBrands.Any(c => c.Name == carBrand))
        {
            await _db.AddAsync(newCarBrand);
            await _db.SaveChangesAsync();
        }
    }
}