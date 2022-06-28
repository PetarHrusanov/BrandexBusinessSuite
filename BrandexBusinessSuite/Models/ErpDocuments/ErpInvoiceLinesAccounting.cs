namespace BrandexSalesAdapter.Models.ErpDocuments;

using Newtonsoft.Json;

using static Common.ErpConstants;

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
    
    
    public string Id { get; set; } 
    
    [JsonProperty(ErpDocuments.ODataId)]
    public string OId { get; set; }

    public int LineNo { get; set; }

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