namespace BrandexBusinessSuite.Models.ErpDocuments;

public class CrmProductPrice
{
    public string Id { get; set; }
    
    public DateTime FromDate { get; set; }
    
    public _Price Price { get; set; }
    
    public _Product Product { get; set; }

    public class _Price
    {
        public decimal Value { get; set; }
    }

    public class _Product
    {
        public string Id { get; set; }
    }
}