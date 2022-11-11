using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpPharmacyCheck
{
    public string? Id { get; set; }
    public string? PartyCode { get; set; }
    public string? PartyId { get; set; }
    
    public bool? IsActive { get; set; }
    
    [JsonProperty("CustomProperty_ADDRES")]
    public ErpCharacteristicValue? Address { get; set; }
    
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DPhoenix")]
    public ErpCharacteristicValue? PhoenixId { get; set; }
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DFarmnet")]
    public ErpCharacteristicValue? PharmnetId { get; set; }
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DSOPHARMA")]
    public ErpCharacteristicValue? SopharmaId { get; set; }
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DSting")]
    public ErpCharacteristicValue? StingId { get; set; }
    
    [JsonProperty("CustomProperty_RETREG")]
    public ErpCharacteristicValueValueId? Region { get; set; }
    
    [JsonProperty("CustomProperty_ВЕРИГА")]
    public ErpCharacteristicValueValueId? PharmacyChain { get; set; }
    [JsonProperty("CustomProperty_КЛАС")]
    public ErpCharacteristicValueValueId? Class { get; set; }
    
    public ErpToPartyAnalysis? ParentParty { get; set; }

    public ErpCharacteristicProductName? LocationName { get; set; }
    
}