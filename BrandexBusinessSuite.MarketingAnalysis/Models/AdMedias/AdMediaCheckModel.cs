using BrandexBusinessSuite.MarketingAnalysis.Data.Enums;

namespace BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;

using MarketingAnalysis.Data.Enums;

public class AdMediaCheckModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public MediaType MediaType { get; set; }
}