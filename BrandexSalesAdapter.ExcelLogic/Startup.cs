namespace BrandexSalesAdapter.ExcelLogic
{
    // Data 
    using BrandexSalesAdapter.ExcelLogic.Data;
    using BrandexSalesAdapter.ExcelLogic.Models;
    using BrandexSalesAdapter.ExcelLogic.Data.Models.ApplicationUserModels;

    // Common Services
    using BrandexSalesAdapter.ExcelLogic.Services;
    using BrandexSalesAdapter.ExcelLogic.Services.Mapping;

    // Business- Specific Services
    using BrandexSalesAdapter.ExcelLogic.Services.Cities;
    using BrandexSalesAdapter.ExcelLogic.Services.Companies;
    using BrandexSalesAdapter.ExcelLogic.Services.Distributor;
    using BrandexSalesAdapter.ExcelLogic.Services.Pharmacies;
    using BrandexSalesAdapter.ExcelLogic.Services.PharmacyChains;
    using BrandexSalesAdapter.ExcelLogic.Services.Products;
    using BrandexSalesAdapter.ExcelLogic.Services.Regions;
    using BrandexSalesAdapter.ExcelLogic.Services.Sales;

    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using BrandexSalesAdapter.ExcelLogic.Data.Seeding;
    using Microsoft.AspNetCore.Identity;
    using BrandexSalesAdapter.ExcelLogic.Models.Map;
    using System;

    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IHostEnvironment hostingEnvironment;

        public Startup(
            IConfiguration configuration,
            IHostEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SpravkiDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<SpravkiDbContext>();

            //services.Configure<CookiePolicyOptions>(
            //    options =>
            //    {
            //        options.CheckConsentNeeded = context => true;
            //        options.MinimumSameSitePolicy = SameSiteMode.None;
            //    });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton(this.configuration);

            services
                .AddAutoMapper(
                    (_, config) => config
                        .AddProfile(new MappingProfile(Assembly.GetCallingAssembly())),
                    Array.Empty<Assembly>());

            services
                //.AddWebService<SpravkiDbContext>(this.configuration)

                // Business Logic 
                .AddTransient<ICitiesService, CitiesService>()
                .AddTransient<ICompaniesService, CompaniesService>()
                .AddTransient<IDistributorService, DistributorService>()
                .AddTransient<IPharmaciesService, PharmaciesService>()
                .AddTransient<IPharmacyChainsService, PharmacyChainsService>()
                .AddTransient<IProductsService, ProductsService>()
                .AddTransient<IRegionsService, RegionsService>()
                .AddTransient<ISalesService, SalesService>()
                .AddTransient<INumbersChecker, NumbersChecker>()

                //.AddControllersWithViews(options => options
                //    .Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
                ;

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
                //app.UseDatabaseErrorPage();
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

            //app.UseCors(options => options
            //        .AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod());

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