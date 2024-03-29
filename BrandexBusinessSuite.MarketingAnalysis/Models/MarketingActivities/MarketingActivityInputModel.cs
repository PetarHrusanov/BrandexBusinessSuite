namespace BrandexBusinessSuite.MarketingAnalysis.Models.MarketingActivities;

using AutoMapper;
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models;

public class MarketingActivityInputModel: IMapFrom<MarketingActivity>
{
    public string Description { get; set; }
        
    public DateTime Date { get; set; }
        
    public int ProductId { get; set; }

    public int AdMediaId { get; set; }
    
    public int MediaTypeId { get; set; }
    
    public decimal Price { get; set; }
    
    public bool Paid { get; set; }

    public virtual void Mapping(Profile mapper)
        => mapper.CreateMap<MarketingActivity, MarketingActivityInputModel>();

}