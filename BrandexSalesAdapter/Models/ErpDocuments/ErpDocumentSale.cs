namespace BrandexSalesAdapter.Models.ErpDocuments;

using Newtonsoft.Json;

public abstract class ErpDocumentSale : ErpDocument
{

    public ErpDocumentSale()
    {
        DocumentCurrency = new ErpCharacteristicId();
        PaymentAccount = new ErpCharacteristicId();
        PaymentType = new ErpCharacteristicId();
        Store = new ErpCharacteristicId();
    }
    
    public ErpCharacteristicId DocumentCurrency { get; set; }
    public ErpCharacteristicId PaymentAccount { get; set; }
    public ErpCharacteristicId PaymentType { get; set; }
    public ErpCharacteristicId Store { get; set; }
    
    
    // public _DocumentCurrency DocumentCurrency { get; set; }
    // public _PaymentAccount PaymentAccount { get; set; }
    // public _PaymentType PaymentType { get; set; }
    //
    // public _Store Store { get; set; }
    //
    // public class _DocumentCurrency
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    //
    // public class _PaymentAccount
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    // public class _PaymentType
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    //
    // public class _Store
    // {
    //     [JsonProperty("@odata.id")]
    //     public string Id { get; set; }
    // }
    
}