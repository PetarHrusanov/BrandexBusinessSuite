using BrandexSalesAdapter.Identity.Data.Models;
using BrandexSalesAdapter.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BrandexSalesAdapter.Identity
{
    using Data;
    
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Identity;
    
    using System.Reflection;
    using System.Text;
    using Data.Seeding;
    using Services.Mapping;
    using BrandexSalesAdapter.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;


    public class Startup
    {
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationUsersDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = true;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("placeholder-key-that-is-long-enough-for-sha256")),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        RequireExpirationTime = false,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true
                    };
                });

            // services.AddUserStorage();
            
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationUsersDbContext>()
            //     .AddDefaultTokenProviders();
            
            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationUsersDbContext>();
            
            // services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationUsersDbContext>();
            
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddEntityFrameworkStores<ApplicationUsersDbContext>();

            services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();

            services.AddControllers();
        }
            // => services
            //     .AddWebService<ApplicationUsersDbContext>(this.Configuration)
            //     .AddUserStorage()
            //     .AddTransient<IDataSeeder, IdentityDataSeeder>()
            //     .AddTransient<IIdentityService, IdentityService>()
            //     .AddTransient<ITokenGeneratorService, TokenGeneratorService>();

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

                // Seed data on application startup
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationUsersDbContext>();

                    // dbContext.Database.Migrate();
                    
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
