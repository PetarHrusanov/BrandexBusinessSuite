namespace BrandexBusinessSuite.MarketingAnalysis.Models.MarketingActivities;

using AutoMapper;
using BrandexBusinessSuite.MarketingAnalysis.Data.Models;

public class MarketingActivityOutputModel
{
    public int Id { get; set; }
        
    public string Description { get; set; }
    
    public string Notes { get; set; }
        
    public string Date { get; set; }
    
    public decimal Price { get; set; }
        
    public string ProductName { get; set; }
    // public virtual Product Product { get; set; }
        
    public string AdMediaName { get; set; }
    // public virtual AdMedia AdMedia { get; set; }
    
    public string AdMediaType { get; set; }
    
    public string CompanyName { get; set; }
    
    public bool Paid { get; set; }
    
    public bool ErpPublished { get; set; }
    
    // public virtual void Mapping(Profile mapper)
    //     => mapper
    //         .CreateMap<MarketingActivity, MarketingActivityOutputModel>();
}