namespace BrandexBusinessSuite.MarketingAnalysis.Models.MarketingActivities;

public class MarketingActivityErpModel
{
    public string Description { get; set; }

    public DateTime Date { get; set; }
        
    public string ProductName { get; set; }

    public string AdMedia { get; set; }
    
    public string MediaType { get; set; }
    
    public decimal Price { get; set; }

    public string CompanyName { get; set; }
    
    public string CompanyErpId { get; set; }
    
    // public string Measurement { get; set; }
    
}