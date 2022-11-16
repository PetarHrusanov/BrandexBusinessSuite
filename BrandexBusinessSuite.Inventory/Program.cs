using BrandexBusinessSuite;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.Services.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<ApplicationSettings>(
        builder.Configuration.GetSection(nameof(ApplicationSettings)),
        config => config.BindNonPublicProperties = true)
    .Configure<ErpUserSettings>(
        builder.Configuration.GetSection(nameof(ErpUserSettings)),
        config => config.BindNonPublicProperties = true);
    // .AddWebService<BrandexSalesAnalysisDbContext>(builder.Configuration)
    // .AddTransient<ICitiesService, CitiesService>()
    // .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
    // .AddTransient<IPharmaciesService, PharmaciesService>()
    // .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
    // .AddTransient<IRegionsService, RegionsService>()
    // .AddTransient<ISalesService, SalesService>()
    // .AddTransient<IProductsService, ProductsService>()
    // .AddTransient<ISeeder, ApplicationDbContextSeeder>();

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();