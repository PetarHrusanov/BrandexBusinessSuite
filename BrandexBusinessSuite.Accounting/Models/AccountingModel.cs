namespace BrandexBusinessSuite.Accounting.Models;

public class AccountingModel
{
    public AccountingModel(string accountingName, string accountingErpNumber, decimal price)
    {
        AccountingName = accountingName;
        AccountingErpNumber = accountingErpNumber;
        Price = price;
    }
    public string AccountingName { get; set; }
    public string AccountingErpNumber { get; set; }
    public decimal Price { get; set; }
}