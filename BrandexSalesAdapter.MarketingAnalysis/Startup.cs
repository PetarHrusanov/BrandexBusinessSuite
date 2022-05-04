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

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebService(env)
            .Initialize();
    }
}