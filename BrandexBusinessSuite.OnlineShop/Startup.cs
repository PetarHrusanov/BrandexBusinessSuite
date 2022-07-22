namespace BrandexBusinessSuite.OnlineShop;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Services.SalesAnalysis;
using Services.Products;
using Data;
using Data.Seeding;
using BrandexBusinessSuite.Services.Data;
using Infrastructure;
using Requests;


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
        services.Configure<ApplicationSettings>(_configuration.GetSection(nameof(ApplicationSettings)),
            config => config.BindNonPublicProperties = true);

        services.Configure<SpeedyUserSettings>(_configuration.GetSection(nameof(SpeedyUserSettings)),
            config => config.BindNonPublicProperties = true);
        
        services.Configure<ErpUserSettings>(_configuration.GetSection(nameof(ErpUserSettings)),
            config => config.BindNonPublicProperties = true);

        services.Configure<WooCommerceSettings>(_configuration.GetSection(nameof(WooCommerceSettings)),
            config => config.BindNonPublicProperties = true);

        services
            .AddWebService<OnlineShopDbContext>(_configuration)
            .AddTransient<ISeeder, ApplicationDbContextSeeder>()
            .AddTransient<IProductsService, ProductsService>()
            .AddTransient<ISalesAnalysisService, SalesAnalysisService>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}