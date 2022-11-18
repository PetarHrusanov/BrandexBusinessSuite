using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.Inventory.Data.Models;

public class Order : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public bool Delivered { get; set; }
    public int MaterialId { get; set; }
    public virtual Material Material { get; set; }
    public int SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}