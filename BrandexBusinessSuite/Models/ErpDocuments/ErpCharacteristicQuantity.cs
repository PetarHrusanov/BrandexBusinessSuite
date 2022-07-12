namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpCharacteristicQuantity
{

    public ErpCharacteristicQuantity()
    {
        
    }

    public ErpCharacteristicQuantity(int value)
    {
        Value = value;
    }
    
    public int Value { get; set; }
    public string Unit { get; set; } = "pcs";
}