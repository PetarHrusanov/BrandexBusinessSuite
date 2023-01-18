namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpOrderLinesAccounting
{
    public ErpOrderLinesAccounting(
        string product,
        decimal lineAmount,
        double quantity,
        string lineStore
    )
    {
        Product = new ErpCharacteristicId(product);
        LineAmount = new ErpCharacteristicLineAmount(lineAmount);
        Quantity =  new ErpCharacteristicValueNumber(quantity);
        LineStore = new ErpCharacteristicId(lineStore);
        PricePerUnit = LineAmount;
    }

    public ErpCharacteristicId Product { get; set; }
    public ErpCharacteristicLineAmount LineAmount { get; set; }
    public ErpCharacteristicValueNumber Quantity { get; set; }
    public ErpCharacteristicId LineStore { get; set; }
    public ErpCharacteristicLineAmount PricePerUnit { get; set; }
    
}