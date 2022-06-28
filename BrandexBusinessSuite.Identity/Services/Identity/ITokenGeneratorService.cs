namespace BrandexBusinessSuite.Identity.Services.Identity
{
    using System.Collections.Generic;
    using Data.Models;

    public interface ITokenGeneratorService
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles = null);
    }
}
