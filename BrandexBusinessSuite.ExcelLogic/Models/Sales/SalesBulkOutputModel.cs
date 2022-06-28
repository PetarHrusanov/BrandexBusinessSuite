namespace BrandexBusinessSuite.ExcelLogic.Models.Sales;

using System.Collections.Generic;
    
public class SalesBulkOutputModel
{
    public string Date { get; set; }

    public string Table { get; set; }
        
    public int[] ErrorsArray { get; set; }

    public Dictionary<int, string> Errors { get; set; }
}