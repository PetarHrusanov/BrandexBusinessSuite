namespace BrandexSalesAdapter.Infrastructure;

using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Data;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseWebService(
        this IApplicationBuilder app, 
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app
            .UseHttpsRedirection()
            .UseRouting()
            .UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod())
            .UseAuthentication()
            .UseAuthorization()
            // .UseEndpoints(endpoints => endpoints
            //     .MapControllers());
            .UseEndpoints(endpoints =>
            {
                    
                // endpoints.MapControllerRoute(
                //     name: "default",
                //     pattern: "{controller=Home}/{action=Index}/{id?}");
                // endpoints.MapRazorPages();
                    
                // endpoints.MapHealthChecks("/health", new HealthCheckOptions
                // {
                //     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                // });
                //
                endpoints.MapControllers();
            });

        return app;
    }

    public static IApplicationBuilder Initialize(
        this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var db = serviceProvider.GetRequiredService<DbContext>();

        db.Database.Migrate();
        
        // new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();

        var seeder = serviceProvider.GetService<ISeeder>();

        seeder?.SeedAsync();

        // foreach (var seeder in seeders)
        // {
        //     seeder.SeedData();
        // }

        return app;
    }
}