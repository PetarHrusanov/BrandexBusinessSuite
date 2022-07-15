namespace BrandexBusinessSuite.Identity.Data;

using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
    
public class ApplicationUsersDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationUsersDbContext(DbContextOptions<ApplicationUsersDbContext> options)
        : base(options)
    {
            
    }
        
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}