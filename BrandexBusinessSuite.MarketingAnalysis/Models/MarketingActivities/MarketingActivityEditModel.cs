namespace BrandexBusinessSuite.MarketingAnalysis.Models.MarketingActivities;

using AutoMapper;
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models;

public class MarketingActivityEditModel: IMapFrom<MarketingActivity>
{
    public int Id { get; set; }
    
    public string Description { get; set; }
    
    public string Notes { get; set; }
        
    public DateTime Date { get; set; }
        
    public int ProductId { get; set; }

    public int AdMediaId { get; set; }
    
    public int MediaTypeId { get; set; }
    
    public decimal Price { get; set; }
    
    public bool Paid { get; set; }
    
    public bool ErpPublished { get; set; }

    public virtual void Mapping(Profile mapper)
        => mapper
            .CreateMap<MarketingActivity, MarketingActivityEditModel>();

}