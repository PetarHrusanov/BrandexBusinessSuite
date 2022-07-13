namespace BrandexBusinessSuite.SalesAnalysis.Models.Sales;

using System;

public class SaleExcelOutputModel
{
    public string Name { get; set; }

    public int ProductId { get; set; }
        
    public double ProductPrice { get; set; }

    public int Count { get; set; }

    public DateTime? Date { get; set; }
        
}