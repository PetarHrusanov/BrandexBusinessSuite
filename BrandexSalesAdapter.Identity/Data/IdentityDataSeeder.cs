namespace BrandexSalesAdapter.Identity.Data
{
    using System.Linq;
    using System.Threading.Tasks;
    using BrandexSalesAdapter.Data;
    using BrandexSalesAdapter.Services.Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Models;

    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IdentitySettings _identitySettings;

        public IdentityDataSeeder(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            IOptions<ApplicationSettings> applicationSettings,
            IOptions<IdentitySettings> identitySettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationSettings = applicationSettings.Value;
            _identitySettings = identitySettings.Value;
        }

        public void SeedData()
        {
            
            // Task
            //     .Run(async () =>
            //     {
            //         var adminRole = new IdentityRole(Constants.AdministratorRoleName);
            //
            //         await this._roleManager.CreateAsync(adminRole);
            //
            //         var adminUser = new User
            //         {
            //             UserName = "admin@crs.com",
            //             Email = "admin@crs.com",
            //             SecurityStamp = "RandomSecurityStamp"
            //         };
            //
            //         await this._userManager.CreateAsync(adminUser, this._identitySettings.AdminPassword);
            //
            //         await this._userManager.AddToRoleAsync(adminUser, Constants.AdministratorRoleName);
            //     })
            //     .GetAwaiter()
            //     .GetResult();
            //
            if (!_roleManager.Roles.Any())
            {
                Task
                    .Run(async () =>
                    {
                        var adminRole = new IdentityRole(Constants.AdministratorRoleName);
            
                        await this._roleManager.CreateAsync(adminRole);
            
                        var adminUser = new User
                        {
                            UserName = "admin@crs.com",
                            Email = "admin@crs.com",
                            SecurityStamp = "RandomSecurityStamp"
                        };
            
                        await this._userManager.CreateAsync(adminUser, this._identitySettings.AdminPassword);
            
                        await this._userManager.AddToRoleAsync(adminUser, Constants.AdministratorRoleName);
                    })
                    .GetAwaiter()
                    .GetResult();
            }

            // if (this.applicationSettings.SeedInitialData)
            // {
            //     Task
            //         .Run(async () =>
            //         {
            //             if (await this.userManager.FindByIdAsync(DataSeederConstants.DefaultUserId) != null)
            //             {
            //                 return;
            //             }
            //
            //             var defaultUser = new User
            //             {
            //                 Id = DataSeederConstants.DefaultUserId,
            //                 UserName = "coolcars@crs.com",
            //                 Email = "coolcars@crs.com"
            //             };
            //
            //             await this.userManager.CreateAsync(defaultUser, DataSeederConstants.DefaultUserPassword);
            //         })
            //         .GetAwaiter()
            //         .GetResult();
            // }
        }
    }
}
