namespace BrandexBusinessSuite.Identity.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class UserWithRoleInputModel
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    
    public string Role { get; set; }

    [Required]
    public string Password { get; set; }
}