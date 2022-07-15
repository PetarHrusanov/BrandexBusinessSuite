namespace BrandexBusinessSuite.Identity.Models.Identity;

public class UserOutputModel
{
    public UserOutputModel(string token)
    {
        Token = token;
    }

    public string Token { get; }
}