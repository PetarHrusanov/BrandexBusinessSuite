using BrandexBusinessSuite.Identity;
using BrandexBusinessSuite.Identity.Data;
using BrandexBusinessSuite.Identity.Data.Models;
using BrandexBusinessSuite.Identity.Data.Seeding;
using BrandexBusinessSuite.Identity.Services.Identity;
using BrandexBusinessSuite.Infrastructure;
using BrandexBusinessSuite.Services.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services  
    .AddTransient<ISeeder, ApplicationDbContextSeeder>()
    .AddTransient<IIdentityService, IdentityService>()
    .AddTransient<ITokenGeneratorService, TokenGeneratorService>();

builder.Services
    .Configure<AdminSettings>(
        builder.Configuration.GetSection(nameof(AdminSettings)),
        config => config.BindNonPublicProperties = true)
    .AddWebService<ApplicationUsersDbContext>(builder.Configuration);

builder.Services
    .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationUsersDbContext>()
    ;

var app = builder.Build();

app.UseWebService(builder.Environment).Initialize();
app.Run();
