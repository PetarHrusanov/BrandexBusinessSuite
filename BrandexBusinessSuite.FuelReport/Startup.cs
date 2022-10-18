using BrandexBusinessSuite.FuelReport.Data.Seeding;
using BrandexBusinessSuite.FuelReport.Services.CarBrands;
using BrandexBusinessSuite.FuelReport.Services.CarModels;
using BrandexBusinessSuite.FuelReport.Services.Cars;
using BrandexBusinessSuite.FuelReport.Services.Drivers;
using BrandexBusinessSuite.FuelReport.Services.Regions;
using BrandexBusinessSuite.FuelReport.Services.RouteLogs;
using BrandexBusinessSuite.Services.Data;

namespace BrandexBusinessSuite.FuelReport;


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
    
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.FuelReport.Data;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostingEnvironment;

    public Startup(
        IConfiguration configuration,
        IHostEnvironment hostingEnvironment)
    {
        _configuration = configuration;
        _hostingEnvironment = hostingEnvironment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddWebService<FuelReportDbContext>(_configuration)
            .AddTransient<ICarBrandService, CarBrandService>()
            .AddTransient<ICarModelService, CarModelService>()
            .AddTransient<ICarService, CarService>()
            .AddTransient<IDriverService, DriverService>()
            .AddTransient<IRegionsService, RegionsService>()
            .AddTransient<IRouteLogService, RouteLogService>()
            .AddTransient<ISeeder, ApplicationDbContextSeeder>();
    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}