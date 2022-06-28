namespace BrandexBusinessSuite.Identity.Data.Seeding;

using System;
using System.Linq;
using System.Threading.Tasks;
    
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using BrandexBusinessSuite.Services.Data;
using Models;

public class AdministratorSeeder : ISeeder
{
    private readonly ApplicationUsersDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;

    public AdministratorSeeder(
        ApplicationUsersDbContext dbContext,
        IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    public void SeedAsync()
    {
        if (_dbContext.Users.Any())
        {
            return;
        }
        
        Task
            .Run(async () =>
            {
                var userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string email = "marketing@brandex.bg";
                string roleName = "Administrator";
                string password = "FabricsFabricsExpensiveLinen";

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                };

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, roleName);

                await _dbContext.SaveChangesAsync();
            })
            .GetAwaiter()
            .GetResult();
        
    }
}