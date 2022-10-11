using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.FuelReport.Data.Models;

public class CarModel : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int CarBrandId { get; set; }
    
    public virtual CarBrand CarBrand { get; set; }
    
    public virtual ICollection<Car> Cars { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}