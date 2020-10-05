namespace BrandexSalesAdapter.ExcelLogic.Models.Identity
{
    using System.ComponentModel.DataAnnotations;

    public class UserInputModel
    {
        [EmailAddress]
        [Required]
        //[MinLength(MinEmailLength)]
        //[MaxLength(MaxEmailLength)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
