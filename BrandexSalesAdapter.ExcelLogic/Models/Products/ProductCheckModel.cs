using System;
namespace BrandexSalesAdapter.ExcelLogic.Models.Products
{
    public class ProductCheckModel
    {
        public int Id { get; set; }

        public int BrandexId { get; set; }

        public int? PharmnetId { get; set; }

        public int? PhoenixId { get; set; }

        public string SopharmaId { get; set; }

        public int? StingId { get; set; }
    }
}
