namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpCharacteristicUnitPrice
{
    public ErpCharacteristicUnitPrice()
    {
        
    }

    public ErpCharacteristicUnitPrice(decimal value)
    {
        Value = value;
    }
    public decimal Value { get; set; }
    
    public string Currencry { get; set; } = "BGN";
}