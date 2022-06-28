namespace BrandexBusinessSuite.Accounting.Models;

using BrandexBusinessSuite.Models.ErpDocuments;
using Newtonsoft.Json;

public class LogisticsProcurementReceivingOrder : ErpDocumentSale
{

    public LogisticsProcurementReceivingOrder()
    {
        // ToParty = new ErpCharacteristicToParty();
        // FromParty = new _FromParty();
        // PurchasePriceList = new _PurchasePriceList();
        // Supplier = new _Supplier();
        // Lines = new List<ErpOrderLinesAccounting>();
        // CurrencyDirectory = new _CurrencyDirectory();

        DocumentType = new ErpCharacteristicId();
        CurrencyDirectory = new ErpCharacteristicId();
        FromParty = new ErpCharacteristicId();
        ToParty = new ErpCharacteristicId();
        PurchasePriceList = new ErpCharacteristicId();
        PaymentType = new ErpCharacteristicId();
        Store = new ErpCharacteristicId();
        Lines = new List<ErpOrderLinesAccounting>();

    }
    public string DocumentDate { get; set; }
    
    public string DocumentNo { get; set; }
    
    public string InvoiceDocumentNo { get; set; }
    
    public ErpCharacteristicId DocumentType { get; set; }
    public ErpCharacteristicId CurrencyDirectory { get; set; }
    public ErpCharacteristicId FromParty { get; set; }
    public ErpCharacteristicId ToParty { get; set; }
    public ErpCharacteristicId PurchasePriceList { get; set; }
    public ErpCharacteristicId Supplier { get; set; }
    public ErpCharacteristicId PaymentType { get; set; }
    
    public ErpCharacteristicId Store { get; set; }

    public List<ErpOrderLinesAccounting> Lines { get; set; }
    
}