namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpLot
{
    public string Id { get; set; }
    public string? ExpiryDate { get; set; }
    public string? ReceiptDate { get; set; }
    
    public _Product? Product { get; set; }

    public class _Product
    {
        public string Id { get; set; }
    }
}