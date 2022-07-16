namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpSalesLines
{
    public ErpSalesLines()
    {
        
    }
    public ErpSalesLines(string productId, decimal quantity, decimal discount, string priceId, decimal unitPrice)
    {
        Product = new ErpCharacteristicId(productId);
        Quantity = new ErpCharacteristicValueDecimal(quantity);
        LineCustomDiscountPercent = discount;
        ProductPrice = new ErpCharacteristicId(priceId);
        UnitPrice = new ErpCharacteristicUnitPrice(unitPrice);
        Lot = null;

    }
    public ErpSalesLines(string productId, decimal quantity, decimal discount, string priceId, decimal unitPrice, string lot)
    {
        Product = new ErpCharacteristicId(productId);
        Quantity = new ErpCharacteristicValueDecimal(quantity);
        LineCustomDiscountPercent = discount;
        ProductPrice = new ErpCharacteristicId(priceId);
        UnitPrice = new ErpCharacteristicUnitPrice(unitPrice);
        // LineAmount = new ErpCharacteristicUnitPrice(unitPrice * quantity);
        Lot = new ErpCharacteristicId(lot);
    }

    public ErpCharacteristicId Product { get; set; }
    public  ErpCharacteristicValueDecimal Quantity { get; set; }
    
    public decimal LineCustomDiscountPercent { get; set; }
    
    public ErpCharacteristicId ProductPrice { get; set; }
    
    public ErpCharacteristicUnitPrice UnitPrice { get; set; }

    public ErpCharacteristicId? Lot { get; set; } = null;
    public ErpCharacteristicId LineStore { get; set; } = new ErpCharacteristicId("Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)");
    public ErpCharacteristicId LineDealType { get; set; } = new ErpCharacteristicId("Finance_Vat_DealTypes(7e483e21-3b9b-489e-bd80-b3373cc9ed3e)");
}