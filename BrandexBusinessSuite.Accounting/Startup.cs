namespace BrandexBusinessSuite.Accounting;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Data;
using Services.Data;
using Data.Seeding;
using Infrastructure;


public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostingEnvironment;

    public Startup(
        IConfiguration configuration,
        IHostEnvironment hostingEnvironment)
    {
        _configuration = configuration;
        _hostingEnvironment = hostingEnvironment;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .Configure<ApplicationSettings>(
                _configuration.GetSection(nameof(ApplicationSettings)), 
                config => config.BindNonPublicProperties = true);
        services
            .Configure<ErpUserSettings>(
                _configuration.GetSection(nameof(ErpUserSettings)), 
                config => config.BindNonPublicProperties = true);
        services
            .AddWebService<AccountingDbContext>(_configuration)
            .AddTransient<ISeeder, ApplicationDbContextSeeder>();
    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebService(env)
            .Initialize();
    }
}