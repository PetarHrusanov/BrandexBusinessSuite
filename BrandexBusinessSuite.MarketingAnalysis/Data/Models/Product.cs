namespace BrandexBusinessSuite.MarketingAnalysis.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class Product :IAuditInfo, IDeletableEntity
{
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public virtual ICollection<MarketingActivity> MarketingActivities { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}