using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpPharmacyCompanyCheck
{
    [JsonProperty("@odata.id")]
    public string Id { get; set; }
    
    public ErpToPartyAnalysis? ParentParty { get; set; }
}