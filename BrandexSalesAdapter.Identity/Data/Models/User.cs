namespace BrandexSalesAdapter.Identity.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
        // public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
        //
        // public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        //
        // public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
    }
    
    
}
