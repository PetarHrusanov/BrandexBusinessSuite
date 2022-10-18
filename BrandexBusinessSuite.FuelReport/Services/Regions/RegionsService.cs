namespace BrandexBusinessSuite.FuelReport.Services.Regions;

using AutoMapper;
using BrandexBusinessSuite.FuelReport.Data;
using BrandexBusinessSuite.FuelReport.Models.CarBrands;
using BrandexBusinessSuite.FuelReport.Models.Regions;
using Microsoft.EntityFrameworkCore;

public class RegionsService : IRegionsService
{
    private readonly FuelReportDbContext _db;
    private readonly IMapper _mapper;

    public RegionsService(FuelReportDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task<RegionOutputModel[]> GetAll()
    {
        return await _mapper.ProjectTo<RegionOutputModel>(_db.Regions).ToArrayAsync();
    }
}