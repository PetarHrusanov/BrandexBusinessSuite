namespace BrandexBusinessSuite.ExcelLogic.Data.Models;

using System;
using System.Collections.Generic;

using BrandexBusinessSuite.Data.Models.Common;

public class Distributor : IAuditInfo, IDeletableEntity
{

    public Distributor()
    {
        Sales = new HashSet<Sale>();
    }
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Sale> Sales { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}