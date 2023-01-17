using BrandexBusinessSuite;
using BrandexBusinessSuite.Accounting.Data;
using BrandexBusinessSuite.Accounting.Data.Seeding;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.Services.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<ApplicationSettings>(
        builder.Configuration.GetSection(nameof(ApplicationSettings)),
        config => config.BindNonPublicProperties = true)
    .Configure<ErpUserSettings>(
        builder.Configuration.GetSection(nameof(ErpUserSettings)),
        config => config.BindNonPublicProperties = true)
    .AddWebService<AccountingDbContext>(builder.Configuration)
    .AddTransient<ISeeder, ApplicationDbContextSeeder>();

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();