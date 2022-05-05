using BrandexSalesAdapter.MarketingAnalysis.Services.AdMedias;

namespace BrandexSalesAdapter.MarketingAnalysis;

using BrandexSalesAdapter.Infrastructure;
using BrandexSalesAdapter.MarketingAnalysis.Data;

public class Startup
{
    public Startup(IConfiguration configuration) => this.Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddWebService<MarketingAnalysisDbContext>(Configuration);

        services
            .AddTransient<IAdMediasService, AdMediasService>();
        
        services.AddCors();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebService(env)
            .Initialize();
    }
}