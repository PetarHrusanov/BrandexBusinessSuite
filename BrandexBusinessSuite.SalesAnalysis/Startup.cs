namespace BrandexBusinessSuite.SalesAnalysis;

using Data;

using Services.Cities;
using Services.Distributor;
using Services.Pharmacies;
using Services.PharmacyChains;
using Services.Products;
using Services.Regions;
using Services.Sales;
using Services.PharmacyCompanies;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
    
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using BrandexBusinessSuite.Services.Data;
using BrandexBusinessSuite.Infrastructure;
using Data.Seeding;


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
            .AddWebService<SalesAnalysisDbContext>(_configuration)
            .AddTransient<ICitiesService, CitiesService>()
            .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
            .AddTransient<IDistributorService, DistributorService>()
            .AddTransient<IPharmaciesService, PharmaciesService>()
            .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
            .AddTransient<IProductsService, ProductsService>()
            .AddTransient<IRegionsService, RegionsService>()
            .AddTransient<ISalesService, SalesService>()
            .AddTransient<ISeeder, ApplicationDbContextSeeder>();

    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}