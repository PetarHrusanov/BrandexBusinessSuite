namespace BrandexBusinessSuite.MarketingAnalysis.Data.Models;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<AdMedia> AdMedias { get; set; }
}