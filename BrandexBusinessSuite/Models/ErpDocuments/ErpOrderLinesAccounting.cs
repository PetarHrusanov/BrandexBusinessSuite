namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpOrderLinesAccounting
{
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
        PricePerUnit = lineAmount;
    }

    public ErpCharacteristicId Product { get; set; }
    public ErpCharacteristicLineAmount LineAmount { get; set; }
    public ErpCharacteristicValueNumber Quantity { get; set; }
    public ErpCharacteristicId LineStore { get; set; }
    public ErpCharacteristicLineAmount PricePerUnit { get; set; }
    
}