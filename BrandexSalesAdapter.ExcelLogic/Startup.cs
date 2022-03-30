namespace BrandexSalesAdapter.ExcelLogic
{
    // Data 
    using Data;
    using Models;
    using Data.Models.ApplicationUserModels;

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
    
    using Data.Seeding;
    using Models.Map;
    using System;

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
            services.AddDbContext<SpravkiDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<SpravkiDbContext>();

            //services.Configure<CookiePolicyOptions>(
            //    options =>
            //    {
            //        options.CheckConsentNeeded = context => true;
            //        options.MinimumSameSitePolicy = SameSiteMode.None;
            //    });

            services.AddControllersWithViews();

            //services.AddControllers(options =>
            //{
            //    options.RespectBrowserAcceptHeader = true; // false by default
            //});

            services.AddRazorPages();

            services.AddSingleton(this._configuration);

            services
                .AddAutoMapper(
                    (_, config) => config
                        .AddProfile(new MappingProfile(Assembly.GetCallingAssembly())),
                    Array.Empty<Assembly>());

            services
                // Business Logic 
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
            services.AddMvc();
            

            services.AddRouting(options => options.LowercaseUrls = true);

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<SpravkiDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app
               //.UseAuthentication()
               //.UseAuthorization()
               .UseRouting()
               .UseCors(options => options
                   .AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod());

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            
            //app.UseCookiePolicy();

            //app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}