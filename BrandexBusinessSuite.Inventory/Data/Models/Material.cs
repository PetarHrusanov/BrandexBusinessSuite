namespace BrandexBusinessSuite.Inventory.Data.Models;

using Enums;
using BrandexBusinessSuite.Data.Models.Common;

public class Material : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpId { get; set; }
    public string PartNumber { get; set; }

    public MaterialType Type { get; set; }
    public MaterialMeasurement Measurement { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    public virtual ICollection<Recipe> Recipes { get; set; } = new HashSet<Recipe>();

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}