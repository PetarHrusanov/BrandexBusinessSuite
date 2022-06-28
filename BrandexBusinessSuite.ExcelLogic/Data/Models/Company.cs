namespace BrandexBusinessSuite.ExcelLogic.Data.Models;

using System;
using System.Collections.Generic;

using BrandexBusinessSuite.Data.Models.Common;

public class Company : IAuditInfo, IDeletableEntity
{
    public Company()
    {
        Pharmacies = new HashSet<Pharmacy>();
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Owner { get; set; }

    public string VAT { get; set; }

    public virtual ICollection<Pharmacy> Pharmacies { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}