namespace BrandexBusinessSuite.Models.ErpDocuments;

public abstract class ErpDocumentSale : ErpDocument
{

    public ErpDocumentSale()
    {
        DocumentCurrency = new ErpCharacteristicId();
        PaymentAccount = new ErpCharacteristicId();
        PaymentType = new ErpCharacteristicId();
        // Store = new ErpCharacteristicId();
    }
    
    public ErpCharacteristicId DocumentCurrency { get; set; } 
    public ErpCharacteristicId PaymentAccount { get; set; }
    public ErpCharacteristicId PaymentType { get; set; }

}