namespace BrandexSalesAdapter.ExcelLogic.Models.Phoenix
{
    using Microsoft.AspNetCore.Http;

    public class PhoenixInputModel
    {
        public string Date { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
