namespace BrandexSalesAdapter.Models.ErpDocuments;

using Newtonsoft.Json;


public class ErpCharacteristicId
{
    [JsonProperty("@odata.id")]
    public string Id { get; set; }
}