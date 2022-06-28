namespace BrandexBusinessSuite.Identity.Data.Seeding;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using BrandexBusinessSuite.Services.Data;

public class ApplicationDbContextSeeder : ISeeder
{

    private readonly ApplicationUsersDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationDbContextSeeder(
        ApplicationUsersDbContext dbContext,
        IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }
        
    public void SeedAsync()
    {
        if (_dbContext == null)
        {
            throw new ArgumentNullException(nameof(_dbContext));
        }

        if (_serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(_serviceProvider));
        }

        var logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger(typeof(ApplicationDbContextSeeder));

        var seeders = new List<ISeeder>
        {
            new RolesSeeder(_dbContext, _serviceProvider),
            new AdministratorSeeder(_dbContext, _serviceProvider),
        };

        foreach (var seeder in seeders)
        {
                
            Task.Run(async () =>
                {
                    seeder.SeedAsync();
                    await _dbContext.SaveChangesAsync();
                    logger.LogInformation($"Seeder {seeder.GetType().Name} done.");
                })
                .GetAwaiter()
                .GetResult();
                
        }
    }
}