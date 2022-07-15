namespace BrandexBusinessSuite.Identity.Models.Identity;

using System.ComponentModel.DataAnnotations;

public class UserInputModel
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}