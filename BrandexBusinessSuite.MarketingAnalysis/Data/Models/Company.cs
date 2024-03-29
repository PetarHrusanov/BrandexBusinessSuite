using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.MarketingAnalysis.Data.Models;

public class Company :IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public string ErpId { get; set; }
    public virtual ICollection<AdMedia> AdMedias { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}