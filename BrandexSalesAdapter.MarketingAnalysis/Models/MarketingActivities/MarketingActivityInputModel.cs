namespace BrandexSalesAdapter.MarketingAnalysis.Models.MarketingActivities;

public class MarketingActivityInputModel
{

    public string Description { get; set; }
        
    public DateTime Date { get; set; }
        
    public int ProductId { get; set; }

    public int AdMediaId { get; set; }
    
    public decimal Price { get; set; }
    
}