namespace BrandexSalesAdapter.ExcelLogic
{
    // Data 
    using Data;
    using Models;

    // Common Services
    using Services;
    using Services.Mapping;

    // Business- Specific Services
    using Services.Cities;
    using Services.Distributor;
    using Services.Pharmacies;
    using Services.PharmacyChains;
    using Services.Products;
    using Services.Regions;
    using Services.Sales;
    using Services.PharmacyCompanies;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    
    using BrandexSalesAdapter.Infrastructure;
    
    using Data.Seeding;

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

            services.AddWebService<SpravkiDbContext>(_configuration);

            // services.AddDbContext<SpravkiDbContext>(
            //     options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
            
            // services.AddControllersWithViews();

            services.AddRazorPages();

            // services.AddSingleton(_configuration);
            
            // services
            //     .AddHttpContextAccessor()
            //     .AddScoped<ICurrentUserService, CurrentUserService>();

            // services
            //     .AddAutoMapper(
            //         (_, config) => config
            //             .AddProfile(new MappingProfile(Assembly.GetCallingAssembly())),
            //         Array.Empty<Assembly>());

            services
                .AddTransient<ICitiesService, CitiesService>()
                .AddTransient<IPharmacyCompaniesService, PharmacyCompaniesService>()
                .AddTransient<IDistributorService, DistributorService>()
                .AddTransient<IPharmaciesService, PharmaciesService>()
                .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
                .AddTransient<IProductsService, ProductsService>()
                .AddTransient<IRegionsService, RegionsService>()
                .AddTransient<ISalesService, SalesService>()
                .AddTransient<INumbersChecker, NumbersChecker>();

            services.AddCors();

            // services.AddRouting(options => options.LowercaseUrls = true);

        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app
                .UseWebService(env)
                .Initialize();
            
            // AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            //
            // // Seed data on application startup
            // using (var serviceScope = app.ApplicationServices.CreateScope())
            // {
            //     var dbContext = serviceScope.ServiceProvider.GetRequiredService<SpravkiDbContext>();
            //
            //     if (env.IsDevelopment())
            //     {
            //         dbContext.Database.Migrate();
            //     }
            //
            //     new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            // }
            //
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else
            // {
            //     app.UseExceptionHandler("/Home/Error");
            //     app.UseHsts();
            // }
            //
            // app
            //     .UseRouting()
            //    .UseCors(options => options
            //        .AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod());
            //
            // app.UseHttpsRedirection();
            //
            // app.UseStaticFiles();
            //
            //
            // app.UseAuthentication();
            // app.UseAuthorization();
            //
            // app.UseEndpoints(endpoints =>
            // {
            //     endpoints.MapControllerRoute(
            //         name: "default",
            //         pattern: "{controller=Home}/{action=Index}/{id?}");
            //     endpoints.MapRazorPages();
            // });
        }
    }
}