namespace BrandexBusinessSuite.Inventory.Data.Models;

using BrandexBusinessSuite.Data.Models.Common;

public class Supplier : IAuditInfo, IDeletableEntity
{
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Contact { get; set; }
    public string VAT { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}