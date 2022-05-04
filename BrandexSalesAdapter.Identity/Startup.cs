namespace BrandexSalesAdapter.Identity
{
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Identity;
    using Data.Models;
    using BrandexSalesAdapter.Infrastructure;


    public class Startup
    {
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddWebService<ApplicationUsersDbContext>(Configuration);
                
            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationUsersDbContext>();

            services
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<ITokenGeneratorService, TokenGeneratorService>();
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebService(env)
                .Initialize();
        }
    }
}
