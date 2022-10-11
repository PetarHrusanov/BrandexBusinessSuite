using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.FuelReport.Data.Models;

public class DriverRegion : IAuditInfo, IDeletableEntity
{
    
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }
    
    public int RegionId { get; set; }
    public Region Region { get; set; }
    
    public bool Active { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}