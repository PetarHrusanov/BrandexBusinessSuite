namespace BrandexSalesAdapter.Models.ErpDocuments;

public class ErpOrderLinesAccounting
{
    // public ErpOrderLinesAccounting()
    // {
    //     Product = new ErpCharacteristicId();
    //     LineAmount = new ErpCharacteristicLineAmount();
    //     LineStore = new ErpCharacteristicId();
    //     // UnitPrice = new ErpCharacteristicQuantityUnit();
    //     Quantity = new ErpCharacteristicValueNumber();
    // }

    public ErpOrderLinesAccounting(
        ErpCharacteristicId product,
        ErpCharacteristicLineAmount lineAmount,
        ErpCharacteristicValueNumber quantity,
        ErpCharacteristicId lineStore
    )
    {
        Product = product;
        LineAmount = lineAmount;
        Quantity = quantity;
        LineStore = lineStore;
    }


    public ErpCharacteristicId Product { get; set; }
    public ErpCharacteristicLineAmount LineAmount { get; set; }
    public ErpCharacteristicValueNumber Quantity { get; set; }
    public ErpCharacteristicId LineStore { get; set; }

    // public ErpCharacteristicQuantityUnit UnitPrice { get; set; }
    
    
    
    

}