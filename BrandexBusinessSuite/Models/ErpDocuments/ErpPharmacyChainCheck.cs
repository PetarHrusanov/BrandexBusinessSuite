namespace BrandexBusinessSuite.Models.ErpDocuments;

using Newtonsoft.Json;

public class ErpPharmacyChainCheck
{
    [JsonProperty("@odata.id")]
    public string Id { get; set; }

    [JsonProperty("CustomProperty_ВЕРИГА")]
    public ErpCharacteristicValueValueId? PharmacyChain { get; set; }
}