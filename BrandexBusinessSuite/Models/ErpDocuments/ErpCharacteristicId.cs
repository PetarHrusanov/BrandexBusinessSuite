namespace BrandexBusinessSuite.Models.ErpDocuments;

using Newtonsoft.Json;


public class ErpCharacteristicId
{

    public ErpCharacteristicId() { }

    public ErpCharacteristicId(string id)
    {
        Id = id;
    }
    
    [JsonProperty("@odata.id")]
    public string? Id { get; set; }
}