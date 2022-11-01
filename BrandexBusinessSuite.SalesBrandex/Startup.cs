namespace BrandexBusinessSuite.SalesBrandex;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Infrastructure;
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) 
        => _configuration = configuration;
    
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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebService(env);
            // .Initialize();
    }
}