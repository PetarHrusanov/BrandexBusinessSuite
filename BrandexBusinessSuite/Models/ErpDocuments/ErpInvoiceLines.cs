namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpInvoiceLines
{
    public ErpCharacteristicProductName ProductDescription { get; set; }
    public ErpCharacteristicQuantity Quantity { get; set; }
    public ErpCharacteristicQuantity QuantityBase { get; set; }
    public ErpCharacteristicUnitPrice LineAmount { get; set; }
    public ErpCharacteristicUnitPrice UnitPrice { get; set; }
    
    public ErpCharacteristicId QuantityUnit { get; set; } = new("General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)");
    public ErpCharacteristicId ParentSalesOrderLine { get; set; }
    public ErpCharacteristicId SalesOrder { get; set; }
    public ErpCharacteristicId InvoiceOrderLine { get; set; }
}