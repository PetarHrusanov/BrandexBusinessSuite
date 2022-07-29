namespace BrandexBusinessSuite.Accounting;

using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using BrandexBusinessSuite.Models;
using Services.Identity;
using Infrastructure;
using Requests;


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
            .AddAutoMapper(
                (_, config) => config
                    .AddProfile(new MappingProfile(Assembly.GetCallingAssembly())),
                Array.Empty<Assembly>());

        JwtBearerEvents events = null;

        var secret = _configuration
            .GetSection(nameof(ApplicationSettings))
            .GetValue<string>(nameof(ApplicationSettings.Secret));

        var key = Encoding.ASCII.GetBytes(secret);

        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                if (events != null)
                {
                    bearer.Events = events;
                }
            });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddControllers();
    }

        
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseWebService(env);
    }
}