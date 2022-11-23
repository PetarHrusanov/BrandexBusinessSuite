namespace BrandexBusinessSuite.Models.ErpDocuments;

using Newtonsoft.Json;

public class ErpCharacteristicValue
{

    public ErpCharacteristicValue() { }

    public ErpCharacteristicValue(string value)
    {
        Value = value;
    }
    public string? Value { get; set; }
}