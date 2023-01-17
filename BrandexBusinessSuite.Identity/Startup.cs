namespace BrandexBusinessSuite.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Services.Identity;
using Data;
using Data.Models;
using Data.Seeding;
using Infrastructure;
using BrandexBusinessSuite.Services.Data;

public class Startup
{
    public Startup(IConfiguration configuration) => this.Configuration = configuration;

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        
        services
            .Configure<AdminSettings>(
                Configuration.GetSection(nameof(AdminSettings)), 
                config => config.BindNonPublicProperties = true);
        
        services.AddWebService<ApplicationUsersDbContext>(Configuration);
                
        services
            .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationUsersDbContext>();

        services
            .AddTransient<ISeeder, ApplicationDbContextSeeder>()
            .AddTransient<IIdentityService, IdentityService>()
            .AddTransient<ITokenGeneratorService, TokenGeneratorService>();
            
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebService(env)
            .Initialize();
    }
}