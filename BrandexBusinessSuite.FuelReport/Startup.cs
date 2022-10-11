using BrandexBusinessSuite.FuelReport.Data.Seeding;
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
            .AddTransient<ISeeder, ApplicationDbContextSeeder>();
        //     .AddTransient<ICitiesService, CitiesService>()
        //     .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
        //     .AddTransient<IDistributorService, DistributorService>()
        //     .AddTransient<IPharmaciesService, PharmaciesService>()
        //     .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
        //     .AddTransient<IProductsService, ProductsService>()
        //     .AddTransient<IRegionsService, RegionsService>()
        //     .AddTransient<ISalesService, SalesService>()
        // // .AddMessaging(_configuration);

    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}