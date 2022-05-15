using System.Reflection;
using BrandexSalesAdapter.Models;

namespace BrandexSalesAdapter.Accounting;


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
    
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using BrandexSalesAdapter.Infrastructure;


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

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        services
            .Configure<ApplicationSettings>(
                _configuration.GetSection(nameof(ApplicationSettings)), 
                config => config.BindNonPublicProperties = true);
        
        services
            .AddAutoMapper(
                (_, config) => config
                    .AddProfile(new MappingProfile(Assembly.GetCallingAssembly())),
                Array.Empty<Assembly>());

        services.AddControllers();
        
        services.AddCors();

    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseWebService(env);
    }
}