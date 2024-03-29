namespace BrandexBusinessSuite.FuelReport.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;
using BrandexBusinessSuite.Data.Models.Common;

public class RouteLog : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal Km { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal MileageStart { get; set; }
    
    [Column(TypeName = "decimal(18,4)")]
    public decimal MileageEnd { get; set; }
    
    public DateTime Date { get; set; }

    public int DriverCarDriverId { get; set; }
    public int DriverCarCarId { get; set; }

    public virtual DriverCar DriverCar { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}