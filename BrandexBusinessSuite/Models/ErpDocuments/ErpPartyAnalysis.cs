using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpPartyAnalysis
{
    public string? PartyCode { get; set; }

    [JsonProperty("CustomProperty_RETREG")]
    public ErpCharacteristicValueValueId Region { get; set; }
    
    public ErpCharacteristicProductName PartyName { get; set; }
    
}