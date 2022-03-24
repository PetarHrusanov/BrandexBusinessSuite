namespace BrandexSalesAdapter.ExcelLogic.Models.Brandex
{
    using Microsoft.AspNetCore.Http;

    public class BrandexInputModel
    {
        //[FromForm(Name = "d")]
        public string Date { get; set; }

        //[FromForm(Name = "d")]
        public string Distributor { get; set; }

        //[FromForm(Name = "ImageFile")]
        public IFormFile ImageFile { get; set; }
    }
}
