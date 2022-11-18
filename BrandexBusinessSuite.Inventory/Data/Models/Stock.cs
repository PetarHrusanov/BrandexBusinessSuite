namespace BrandexBusinessSuite.Inventory.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class Stock : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    
    public int? ProductId { get; set; }
    public virtual Product? Product { get; set; }
    
    public int? MaterialId { get; set; }
    public virtual Material? Material { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}