using BrandexBusinessSuite.MarketingAnalysis.Data.Models;

namespace BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;

public class AdMediaCheckModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public MediaType MediaType { get; set; }
}