namespace BrandexBusinessSuite.MarketingAnalysis.Data.Models;

public class MediaType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<MarketingActivity> MarketingActivities { get; set; }

}