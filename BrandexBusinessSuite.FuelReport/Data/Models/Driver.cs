namespace BrandexBusinessSuite.FuelReport.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class Driver : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string LastName { get; set; }

    public string UserId { get; set; }
    
    public bool Active { get; set; }
    
    public virtual ICollection<DriverCar> DriverCars { get; set; }
    
    public virtual ICollection<DriverRegion> DriverRegions { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}