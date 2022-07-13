namespace BrandexBusinessSuite.SalesAnalysis.Models.Sales;

using System;

public class SaleInputModel
{
    public int PharmacyId { get; set; }

    public int ProductId { get; set; }

    public int? DistributorId { get; set; }

    public DateTime Date { get; set; }

    public int Count { get; set; }
}