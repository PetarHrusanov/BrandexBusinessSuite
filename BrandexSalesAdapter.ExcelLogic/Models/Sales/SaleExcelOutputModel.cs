namespace BrandexSalesAdapter.ExcelLogic.Models.Sales
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Models.Products;
    
    public class SaleExcelOutputModel
    {
        public string Name { get; set; }

        public int ProductId { get; set; }
        
        public double ProductPrice { get; set; }

        public int Count { get; set; }

        public DateTime? Date { get; set; }
        
    }
}
