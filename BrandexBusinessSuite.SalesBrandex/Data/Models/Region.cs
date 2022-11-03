using BrandexBusinessSuite.Data.Models.Common;

namespace BrandexBusinessSuite.SalesBrandex.Data.Models;

public class Region : IAuditInfo, IDeletableEntity
{
    public Region()
    {

        Pharmacies = new HashSet<Pharmacy>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string ErpId { get; set; }

    public virtual ICollection<Pharmacy> Pharmacies { get; set; }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}