namespace BrandexBusinessSuite.FuelReport.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;
using BrandexBusinessSuite.Data.Models.Common;

public class Car : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    
    public string Registration { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal Mileage { get; set; }
    
    public bool Active { get; set; }
    
    public int CarModelId { get; set; }
    public virtual CarModel CarModel { get; set; }
    
    public virtual ICollection<DriverCar> DriverCars { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}