namespace BrandexBusinessSuite.Models.ErpDocuments;

using Newtonsoft.Json;

public class CrmCustomer
{
    public string Id { get; set; }
    
    [JsonProperty("CustomProperty_GRAD_u002DKLIENT")]
    public ErpCharacteristicValueValueId? City { get; set; }
    
    [JsonProperty("CustomProperty_Klas_u0020Klient")]
    public ErpCharacteristicValueValueId? Class { get; set; } = null;
    
    [JsonProperty("CustomProperty_STOR3")]
    public ErpCharacteristicValueValueId? PharmacyChain { get; set; } = null;
    
    public ErpPartyAnalysis? Party { get; set; }
    
}