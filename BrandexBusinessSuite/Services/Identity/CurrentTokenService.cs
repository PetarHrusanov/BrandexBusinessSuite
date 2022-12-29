namespace BrandexBusinessSuite.Services.Identity;

public class CurrentTokenService : ICurrentTokenService
{
    private string currentToken;

    public string Get() => currentToken;

    public void Set(string token) => currentToken = token;
}