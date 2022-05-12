namespace BrandexSalesAdapter.ExcelLogic;

// Data 
using Data;

// Common Services
using Services;

// Business- Specific Services
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

using BrandexSalesAdapter.Infrastructure;


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

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddWebService<SpravkiDbContext>(_configuration);

        services
            .AddTransient<ICitiesService, CitiesService>()
            .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
            .AddTransient<IDistributorService, DistributorService>()
            .AddTransient<IPharmaciesService, PharmaciesService>()
            .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
            .AddTransient<IProductsService, ProductsService>()
            .AddTransient<IRegionsService, RegionsService>()
            .AddTransient<ISalesService, SalesService>()
            .AddTransient<INumbersChecker, NumbersChecker>();

        services.AddCors();

    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}