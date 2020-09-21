namespace BrandexSalesAdapter.ExcelLogic.Models.Brandex
{
    using Microsoft.AspNetCore.Http;

    public class BrandexInputModel
    {
        public string Date { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
