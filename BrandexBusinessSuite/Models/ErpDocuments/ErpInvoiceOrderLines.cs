using Newtonsoft.Json;

namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpInvoiceOrderLines
{
    [JsonProperty("@odata.id")]
    public string Id { get; set; }
    public ErpCharacteristicProductName ProductDescription { get; set; }
    public ErpCharacteristicValue Quantity { get; set; }
    
    public ErpCharacteristicValue QuantityBase { get; set; }
    public ErpCharacteristicUnitPrice LineAmount { get; set; }
    public ErpCharacteristicUnitPrice UnitPrice { get; set; }
    
    public decimal LineCustomDiscountPercent { get; set; }
    
}