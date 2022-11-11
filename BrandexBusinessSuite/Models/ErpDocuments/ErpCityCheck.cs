using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpCityCheck
{
    [JsonProperty("@odata.id")]
    public string Id { get; set; }
    
    [JsonProperty("CustomProperty_GRAD_u002DKLIENT")]
    public ErpCharacteristicValueValueId? City { get; set; }
    
    public ErpToPartyAnalysis? Party { get; set; }
}