namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpCharacteristicValueDecimal
{
    
    public ErpCharacteristicValueDecimal(decimal value)
    {
        Value = value;
    }
    
    public decimal Value { get; set; }
}