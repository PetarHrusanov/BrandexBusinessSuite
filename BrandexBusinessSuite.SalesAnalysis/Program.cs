using BrandexBusinessSuite;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.SalesAnalysis.Data;
using BrandexBusinessSuite.SalesAnalysis.Data.Seeding;
using BrandexBusinessSuite.SalesAnalysis.Services.Cities;
using BrandexBusinessSuite.SalesAnalysis.Services.Distributor;
using BrandexBusinessSuite.SalesAnalysis.Services.Pharmacies;
using BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;
using BrandexBusinessSuite.SalesAnalysis.Services.PharmacyCompanies;
using BrandexBusinessSuite.SalesAnalysis.Services.Products;
using BrandexBusinessSuite.SalesAnalysis.Services.Regions;
using BrandexBusinessSuite.SalesAnalysis.Services.Sales;
using BrandexBusinessSuite.Services.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebService<SalesAnalysisDbContext>(builder.Configuration)
    .Configure<ErpUserSettings>(
        builder.Configuration.GetSection(nameof(ErpUserSettings)), 
        config => config.BindNonPublicProperties = true)
    .AddTransient<ICitiesService, CitiesService>()
    .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
    .AddTransient<IDistributorService, DistributorService>()
    .AddTransient<IPharmaciesService, PharmaciesService>()
    .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
    .AddTransient<IProductsService, ProductsService>()
    .AddTransient<IRegionsService, RegionsService>()
    .AddTransient<ISalesService, SalesService>()
    .AddTransient<ISeeder, ApplicationDbContextSeeder>();

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();

