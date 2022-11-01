using BrandexBusinessSuite.MarketingAnalysis.Services.MediaTypes;

namespace BrandexBusinessSuite.MarketingAnalysis;

using Services.AdMedias;
using Services.Products;
using Infrastructure;
using Data;

using BrandexBusinessSuite.MarketingAnalysis.Data.Seeding;
using BrandexBusinessSuite.MarketingAnalysis.Services.Companies;
using BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;
using BrandexBusinessSuite.Services.Data;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddWebService<MarketingAnalysisDbContext>(_configuration);

        services.Configure<ErpUserSettings>(_configuration.GetSection(nameof(ErpUserSettings)),
            config => config.BindNonPublicProperties = true);

        services
            .AddTransient<IAdMediasService, AdMediasService>()
            .AddTransient<IProductsService, ProductsService>()
            .AddTransient<IMarketingActivitesService, MarketingActivitiesService>()
            .AddTransient<ICompaniesService, CompaniesService>()
            .AddTransient<IMediaTypesService, MediaTypesService>()
            .AddTransient<ISeeder, ApplicationDbContextSeeder>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env)
            .Initialize();
    }
}