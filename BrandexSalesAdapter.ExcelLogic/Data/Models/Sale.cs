﻿namespace BrandexSalesAdapter.ExcelLogic.Data.Models;

using System;

using BrandexSalesAdapter.Data.Models.Common;

public class Sale : IAuditInfo, IDeletableEntity
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }
    public virtual Pharmacy Pharmacy { get; set; }

    public int ProductId { get; set; }
    public virtual Product Product { get; set; }

    public int DistributorId { get; set; }
    public virtual Distributor Distributor { get; set; }

    public DateTime Date { get; set; }

    public int Count { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
}