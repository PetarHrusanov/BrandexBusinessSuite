using System;
namespace BrandexSalesAdapter.ExcelLogic.Models.Sales
{
    public class SaleExcelOutputModel
    {
        public string Name { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }

        public DateTime? Date { get; set; }
    }
}
