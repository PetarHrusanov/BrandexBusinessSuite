using System.ComponentModel.DataAnnotations;

namespace BrandexBusinessSuite.FuelReport.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class DriverCar : IAuditInfo, IDeletableEntity
{
    public int DriverId { get; set; }
    public virtual Driver Driver { get; set; }
    
    public int CarId { get; set; }
    public virtual Car Car { get; set; }

    public bool Active { get; set; }
    
    public virtual ICollection<RouteLog> RouteLogs { get; set; } = new List<RouteLog>();

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}