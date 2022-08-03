using BrandexBusinessSuite.MarketingAnalysis.Data.Seeding;
using BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;
using BrandexBusinessSuite.Services.Data;

namespace BrandexBusinessSuite.MarketingAnalysis;

using Services.AdMedias;
using Services.Products;
using Infrastructure;
using Data;

public class Startup
{
    public Startup(IConfiguration configuration) => this.Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddWebService<MarketingAnalysisDbContext>(Configuration);

        services
            .AddTransient<IAdMediasService, AdMediasService>()
            .AddTransient<IProductsService, ProductsService>()
            .AddTransient<IMarketingActivitesService, MarketingActivitiesService>()
            .AddTransient<ISeeder, ApplicationDbContextSeeder>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}