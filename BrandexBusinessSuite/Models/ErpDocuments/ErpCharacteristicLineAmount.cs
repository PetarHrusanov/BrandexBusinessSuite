namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpCharacteristicLineAmount
{
    public ErpCharacteristicLineAmount(decimal value)
    {
        Value = value;
    }
    public decimal Value { get; set; }

    public string Currencry { get; set; } = "BGN";
    
}