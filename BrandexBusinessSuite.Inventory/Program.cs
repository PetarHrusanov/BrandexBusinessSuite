using BrandexBusinessSuite;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.Inventory.Data;
using BrandexBusinessSuite.Inventory.Services.Materials;
using BrandexBusinessSuite.Inventory.Services.Orders;
using BrandexBusinessSuite.Inventory.Services.Products;
using BrandexBusinessSuite.Inventory.Services.Suppliers;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<ApplicationSettings>(
        builder.Configuration.GetSection(nameof(ApplicationSettings)),
        config => config.BindNonPublicProperties = true)
    .Configure<ErpUserSettings>(
        builder.Configuration.GetSection(nameof(ErpUserSettings)),
        config => config.BindNonPublicProperties = true)
    .AddWebService<InventoryDbContext>(builder.Configuration)
    .AddTransient<IMaterialsService, MaterialsService>()
    .AddTransient<IProductsService, ProductsService>()
    .AddTransient<ISuppliersService, SuppliersService>()
    .AddTransient<IOrdersService, OrdersService>()
    // .AddTransient<ISeeder, ApplicationDbContextSeeder>();
    ;

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();