namespace BrandexSalesAdapter.ExcelLogic.Infrastructure.Extensions
{
    using BrandexSalesAdapter.ExcelLogic.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<SpravkiDbContext>().Database.Migrate();
            }

            return app;
        }
    }
}