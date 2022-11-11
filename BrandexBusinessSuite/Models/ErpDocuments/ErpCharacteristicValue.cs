namespace BrandexBusinessSuite.Models.ErpDocuments;

using Newtonsoft.Json;

public class ErpCharacteristicValue
{

    public ErpCharacteristicValue() { }

    public ErpCharacteristicValue(string value)
    {
        Value = value;
    }
    // [JsonProperty("@odata.id")]
    public string? Value { get; set; }
}