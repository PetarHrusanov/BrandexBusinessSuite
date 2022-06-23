namespace BrandexSalesAdapter.Accounting.Models;

using BrandexSalesAdapter.Models.ErpDocuments;

public class LogisticsProcurementPurchaseInvoice : ErpDocumentSale
{
    public LogisticsProcurementPurchaseInvoice()
    {
        CurrencyDirectory = new ErpCharacteristicId();
        FromParty = new ErpCharacteristicId();
        ToParty = new ErpCharacteristicId();
        PaymentAccount = new ErpCharacteristicId();
        PaymentType = new ErpCharacteristicId();
        PurchasePriceList = new ErpCharacteristicId();
        ReceivingOrder = new ErpCharacteristicId();
        Supplier = new ErpCharacteristicId();
        Lines = new List<ErpInvoiceLinesAccounting>();
    }
    public ErpCharacteristicId CurrencyDirectory { get; set; }
    public ErpCharacteristicId FromParty { get; set; }
    public ErpCharacteristicId ToParty { get; set; }
    public ErpCharacteristicId PaymentAccount { get; set; }
    public ErpCharacteristicId PaymentType { get; set; }
    public ErpCharacteristicId PurchasePriceList { get; set; }
    public ErpCharacteristicId ReceivingOrder { get; set; }
    public ErpCharacteristicId Supplier { get; set; }
    
    public List<ErpInvoiceLinesAccounting> Lines { get; set; }

}