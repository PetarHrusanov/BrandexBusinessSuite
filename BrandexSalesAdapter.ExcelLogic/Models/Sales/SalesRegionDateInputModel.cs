namespace BrandexSalesAdapter.ExcelLogic.Models.Sales
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SalesRegionDateInputModel
    {
        [Required]
        public int RegionId { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
