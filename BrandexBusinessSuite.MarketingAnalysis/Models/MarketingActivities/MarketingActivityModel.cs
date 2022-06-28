namespace BrandexBusinessSuite.MarketingAnalysis.Models.MarketingActivities;

public class MarketingActivityModel
{
    public int Id { get; set; }
        
    public string Description { get; set; }
        
    public DateTime Date { get; set; }
    
    public decimal Price { get; set; }
        
    public int ProductId { get; set; }
    // public virtual Product Product { get; set; }
        
    public int AdMediaId { get; set; }
    // public virtual AdMedia AdMedia { get; set; }
}