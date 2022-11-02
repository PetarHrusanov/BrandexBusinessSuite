﻿namespace BrandexBusinessSuite.SalesBrandex.Data.Models;

using System;
using System.Collections.Generic;

using BrandexBusinessSuite.Data.Models.Common;

public class PharmacyChain : IAuditInfo, IDeletableEntity
{
    public PharmacyChain()
    {
        Pharmacies = new HashSet<Pharmacy>();
    }
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Pharmacy> Pharmacies { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}