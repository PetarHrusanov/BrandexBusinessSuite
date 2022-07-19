namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpInvoiceLines
{

    public ErpInvoiceLines(ErpInvoiceOrderLines invoiceOrderLine, ErpSalesLinesOutput linesOutput, string documentId)
    {
        ProductDescription = invoiceOrderLine.ProductDescription;
        Quantity = new ErpCharacteristicQuantity(Convert.ToInt16(invoiceOrderLine.Quantity.Value));
        QuantityBase = new ErpCharacteristicQuantity(Convert.ToInt16(invoiceOrderLine.QuantityBase.Value));
        StandardQuantityBase = new ErpCharacteristicQuantity(Convert.ToInt16(invoiceOrderLine.QuantityBase.Value));
        LineAmount = invoiceOrderLine.LineAmount;
        SalesOrderAmount = invoiceOrderLine.LineAmount.Value;
        UnitPrice = invoiceOrderLine.UnitPrice;
        ParentSalesOrderLine = new ErpCharacteristicId(linesOutput.Id);
        SalesOrder = new ErpCharacteristicId(documentId);
        InvoiceOrderLine = new ErpCharacteristicId(invoiceOrderLine.Id);
        LineNo = linesOutput.LineNo;
        Product = linesOutput.Product;

    }
    public ErpCharacteristicProductName ProductDescription { get; set; }
    public ErpCharacteristicQuantity Quantity { get; set; }
    public ErpCharacteristicQuantity QuantityBase { get; set; }
    public ErpCharacteristicQuantity StandardQuantityBase { get; set; }
    public ErpCharacteristicUnitPrice LineAmount { get; set; }
    public ErpCharacteristicUnitPrice UnitPrice { get; set; }
    
    public ErpCharacteristicId QuantityUnit { get; set; } = new("General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)");
    public ErpCharacteristicId ParentSalesOrderLine { get; set; }
    public ErpCharacteristicId SalesOrder { get; set; }
    public ErpCharacteristicId InvoiceOrderLine { get; set; }
    public ErpCharacteristicId Product { get; set; }
    
    public int LineNo { get; set; }

    public decimal SalesOrderAmount { get; set; }
}