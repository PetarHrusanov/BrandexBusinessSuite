using BrandexBusinessSuite;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.MarketingAnalysis.Data.Seeding;
using BrandexBusinessSuite.MarketingAnalysis.Services.AdMedias;
using BrandexBusinessSuite.MarketingAnalysis.Services.Companies;
using BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;
using BrandexBusinessSuite.MarketingAnalysis.Services.MediaTypes;
using BrandexBusinessSuite.MarketingAnalysis.Services.Products;
using BrandexBusinessSuite.Services.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<ErpUserSettings>(
        builder.Configuration.GetSection(nameof(ErpUserSettings)),
        config => config.BindNonPublicProperties = true)
    .AddTransient<IAdMediasService, AdMediasService>()
    .AddTransient<IProductsService, ProductsService>()
    .AddTransient<IMarketingActivitesService, MarketingActivitiesService>()
    .AddTransient<ICompaniesService, CompaniesService>()
    .AddTransient<IMediaTypesService, MediaTypesService>()
    .AddTransient<ISeeder, ApplicationDbContextSeeder>();

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();
