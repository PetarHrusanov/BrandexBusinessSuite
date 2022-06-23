namespace BrandexSalesAdapter.Models.ErpDocuments;

public class ErpOrderLinesAccounting
{
    public ErpOrderLinesAccounting()
    {
        Product = new ErpCharacteristicId();
        LineAmount = new ErpCharacteristicLineAmount();
        LineStore = new ErpCharacteristicId();
        // UnitPrice = new ErpCharacteristicQuantityUnit();
        Quantity = new ErpCharacteristicValueNumber();
    }
    
    
    public ErpCharacteristicId Product { get; set; }
    
    public ErpCharacteristicLineAmount LineAmount { get; set; }

    public ErpCharacteristicId LineStore { get; set; }
    // {
    //     set => LineStore.Id = "100447ff-44f4-4799-a4c2-7c9b22fb0aaa";
    //     get => LineStore;
    // }
    
    // public ErpCharacteristicQuantityUnit UnitPrice { get; set; }
    
    public ErpCharacteristicValueNumber Quantity { get; set; }
    
    
    

}