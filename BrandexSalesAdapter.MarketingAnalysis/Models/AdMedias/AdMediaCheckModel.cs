namespace BrandexSalesAdapter.MarketingAnalysis.Models.AdMedias;

using BrandexSalesAdapter.MarketingAnalysis.Data.Enums;

public class AdMediaCheckModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public MediaType MediaType { get; set; }
}