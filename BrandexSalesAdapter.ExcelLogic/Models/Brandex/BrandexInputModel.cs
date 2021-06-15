namespace BrandexSalesAdapter.ExcelLogic.Models.Brandex
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BrandexInputModel
    {
        [FromForm(Name = "Date")]
        public string Date { get; set; }

        [FromForm(Name = "ImageFile")]
        public IFormFile ImageFile { get; set; }
    }
}
