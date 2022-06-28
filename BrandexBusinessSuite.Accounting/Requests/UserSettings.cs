namespace BrandexBusinessSuite.Accounting.Requests;

public class UserSettings
{
    public string MarketingAccount { get; private set; }
    public string MarketingPassword{ get; private set; }
    
    public string AccountingAccount  {get; private set; }
    public string AccountingPassword  {get; private set; }
    
}