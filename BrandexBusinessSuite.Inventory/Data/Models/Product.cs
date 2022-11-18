using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.Inventory.Data.Models;

public class Product : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpId { get; set; }
    public string PartNumber { get; set; }
    
    public virtual ICollection<Recipe> Recipes { get; set; } = new HashSet<Recipe>();
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}