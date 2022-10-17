using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.FuelReport.Data.Models;

public class Region : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual ICollection<DriverRegion> DriverRegions { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}