namespace BrandexBusinessSuite.SalesBrandex.Data.Models;

using System;
using System.Collections.Generic;

using BrandexBusinessSuite.Data.Models.Common;
using Enums;

public class Pharmacy : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }

    public int BrandexId { get; set; }
    
    public string ErpId { get; set; }

    public string Name { get; set; }

    public string PartyCode { get; set; } 

    public PharmacyClass? PharmacyClass{ get;set; }

    public int? CompanyId { get; set; }
    public virtual Company Company { get; set; }

    public int? PharmacyChainId { get; set; }
    public virtual PharmacyChain PharmacyChain { get; set; }

    public string Address { get; set; }

    public int? CityId { get; set; }
    public City City { get; set; }

    public int? RegionId { get; set; }
    public virtual Region Region { get; set; }   

    public virtual ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}