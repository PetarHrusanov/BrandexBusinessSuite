namespace BrandexBusinessSuite.FuelReport.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class CarBrand : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual ICollection<CarModel> CarModels { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}