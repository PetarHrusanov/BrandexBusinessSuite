using Newtonsoft.Json;

namespace BrandexSalesAdapter.Models.ErpDocuments;

public class ErpInvoiceLinesAccounting
{
    public ErpInvoiceLinesAccounting()
    {
        Product = new ErpCharacteristicId();
        Quantity = new ErpCharacteristicValueNumber();
        ProductName = new ErpCharacteristicProductName();
        StandardQuantityBase = new ErpCharacteristicValueNumber();
        UnitPrice = new ErpCharacteristicUnitPrice();
        LineAmount = new ErpCharacteristicUnitPrice();
        PurchaseInvoice = new ErpCharacteristicId();
        QuantityUnit = new ErpCharacteristicId();
        ReceivingOrderLine = new ErpCharacteristicId();
        
        CustomProperty_Продукт_u002Dпокупки = new ErpCharacteristicValueDescriptionBg();
        CustomProperty_ВРМ_u002Dпокупки = new ErpCharacteristicValueDescriptionBg();

    }

    // [JsonProperty("@odata.context")]
    // public string Entity { get; set; } =
    //     "https: //brandexbg.my.erp.net/api/domain/odata/$metadata#Logistics_Procurement_PurchaseInvoiceLines/$entity";
    
    public string Id { get; set; } 
    
    [JsonProperty("@odata.id")]
    public string OId { get; set; }
    
    public string DeliveryTermsCode { get; set; }
    
    public string IntrastatApplyDate { get; set; }
    
    public string IntrastatTransactionNatureCode { get; set; }
    
    public string IntrastatTransportModeCode { get; set; }
    
    public int LineNo { get; set; }
    
    public string Notes { get; set; }
    
    public string IntrastatDestinationRegion { get; set; }
    
    public string IntrastatTransportCountry { get; set; }
    
    public string LineCostCenter { get; set; }
    
    public string LineDealType { get; set; }
    
    public string OriginCountry { get; set; }
    
    public string SaleLineDealType { get; set; }
    
    
    public ErpCharacteristicProductName ProductName { get; set; }
    public ErpCharacteristicValueNumber Quantity { get; set; }
    
    public ErpCharacteristicValueNumber QuantityBase { get; set; }
    public ErpCharacteristicValueNumber StandardQuantityBase { get; set; }
    public ErpCharacteristicUnitPrice UnitPrice { get; set; }
    public ErpCharacteristicUnitPrice LineAmount { get; set; }
    
    
    public ErpCharacteristicId Product { get; set; }
    public ErpCharacteristicId PurchaseInvoice { get; set; }
    public ErpCharacteristicId QuantityUnit { get; set; }
    public ErpCharacteristicId ReceivingOrderLine { get; set; }

    public ErpCharacteristicValueDescriptionBg CustomProperty_Продукт_u002Dпокупки { get; set; }
    
    public ErpCharacteristicValueDescriptionBg CustomProperty_ВРМ_u002Dпокупки { get; set; }

    
}