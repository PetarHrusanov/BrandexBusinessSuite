namespace BrandexSalesAdapter.Identity.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class UserWithRoleInputModel
{
    [EmailAddress]
    [Required]
    // [MinLength(MinEmailLength)]
    // [MaxLength(MaxEmailLength)]
    public string Email { get; set; }
    
    public string Role { get; set; }

    [Required]
    public string Password { get; set; }
}