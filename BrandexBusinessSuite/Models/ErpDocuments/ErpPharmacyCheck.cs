using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpPharmacyCheck
{
    public string? Id { get; set; }
    public string? PartyCode { get; set; }
    public string? PartyId { get; set; }
    
    [JsonProperty("CustomProperty_ADDRES")]
    public ErpCharacteristicValue? Address { get; set; }
    
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DPhoenix")]
    public ErpCharacteristicValue? PhoenixId { get; set; }
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DFarmnet")]
    public ErpCharacteristicValue? PharmnetId { get; set; }
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DSOPHARMA")]
    public ErpCharacteristicValue? SopharmaId { get; set; }
    [JsonProperty("CustomProperty_ID_u002DA_u002DKI_u002DSting")]
    public ErpCharacteristicValue? StringId { get; set; }
    
    public ErpCharacteristicProductName? LocationName { get; set; }
    
}