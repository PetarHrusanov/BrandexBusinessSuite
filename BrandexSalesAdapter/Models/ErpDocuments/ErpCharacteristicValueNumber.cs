namespace BrandexSalesAdapter.Models.ErpDocuments;

public class ErpCharacteristicValueNumber
{

    public ErpCharacteristicValueNumber()
    {
        
    }

    public ErpCharacteristicValueNumber(double value)
    {
        Value = value;
    }
    public double Value { get; set; }
}