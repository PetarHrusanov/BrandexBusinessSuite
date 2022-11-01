namespace BrandexBusinessSuite.Models.ErpDocuments;

public class ErpSalesLineAnalysis
{
    public string Id { get; set; }
    
    public ErpCharacteristicLineAmount LineAmount { get; set; }
    
    public ErpCharacteristicValueDecimal Quantity { get; set; }
    
    public ErpProductAnalysis Product { get; set; }
}