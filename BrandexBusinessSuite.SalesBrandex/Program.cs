using BrandexBusinessSuite;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.SalesBrandex.Data;
using BrandexBusinessSuite.SalesBrandex.Data.Seeding;
using BrandexBusinessSuite.SalesBrandex.Services.Cities;
using BrandexBusinessSuite.SalesBrandex.Services.Pharmacies;
using BrandexBusinessSuite.SalesBrandex.Services.PharmacyChains;
using BrandexBusinessSuite.SalesBrandex.Services.PharmacyCompanies;
using BrandexBusinessSuite.SalesBrandex.Services.Products;
using BrandexBusinessSuite.SalesBrandex.Services.Regions;
using BrandexBusinessSuite.SalesBrandex.Services.Sales;
using BrandexBusinessSuite.Services.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<ApplicationSettings>(
        builder.Configuration.GetSection(nameof(ApplicationSettings)), 
        config => config.BindNonPublicProperties = true)
    .Configure<ErpUserSettings>(
        builder.Configuration.GetSection(nameof(ErpUserSettings)), 
        config => config.BindNonPublicProperties = true)
    .AddWebService<BrandexSalesAnalysisDbContext>(builder.Configuration)
    .AddTransient<ICitiesService, CitiesService>()
    .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
    .AddTransient<IPharmaciesService, PharmaciesService>()
    .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
    .AddTransient<IRegionsService, RegionsService>()
    .AddTransient<ISalesService, SalesService>()
    .AddTransient<IProductsService, ProductsService>()
    .AddTransient<ISeeder, ApplicationDbContextSeeder>();

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();

