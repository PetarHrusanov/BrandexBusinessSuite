using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpSalesLinesOutput
{

    [JsonProperty("@odata.id")]
    public string? Id { get; set; }
    
    public int LineNo { get; set; }
    
    public ErpCharacteristicId Product { get; set; }
}