namespace BrandexBusinessSuite.SalesBrandex.Data.Models;

using System;

using BrandexBusinessSuite.Data.Models.Common;
using System.Collections.Generic;

public class Product : IAuditInfo, IDeletableEntity
{
    public Product()
    {
        Sales = new HashSet<Sale>();
    }

    public int Id { get; set; }
    
    public string ErpId { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public int BrandexId { get; set; }
    public double Price { get; set; }

    public double DiscountPrice
        => Price * 0.88;

    public virtual ICollection<Sale> Sales { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}