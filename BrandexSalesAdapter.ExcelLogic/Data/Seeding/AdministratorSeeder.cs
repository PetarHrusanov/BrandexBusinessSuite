namespace BrandexSalesAdapter.ExcelLogic.Data.Seeding
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Data.Models.ApplicationUserModels;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class AdministratorSeeder : ISeeder
    {
        public AdministratorSeeder()
        {
        }

        public async Task SeedAsync(SpravkiDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Users.Any())
            {
                return;
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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

            await dbContext.SaveChangesAsync();
        }
    }
}
