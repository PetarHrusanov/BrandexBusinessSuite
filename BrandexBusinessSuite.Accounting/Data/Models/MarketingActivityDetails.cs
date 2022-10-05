using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.Accounting.Data.Models;

public class MarketingActivityDetails : IAuditInfo, IDeletableEntity
{
    
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Subject { get; set; }
    
    public string PartyId { get; set; }
    
    public string Measure { get; set; }
    
    public string Type { get; set; }
    
    public string Media { get; set; }
    
    public string PublishType { get; set; }
    
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}