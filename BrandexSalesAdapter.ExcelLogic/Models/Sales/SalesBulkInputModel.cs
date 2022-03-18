namespace BrandexSalesAdapter.ExcelLogic.Models.Sales
{
    using Microsoft.AspNetCore.Http;
    
    public class SalesBulkInputModel
    {
        public string Date { get; set; }
        
        public string Distributor { get; set;}
        
        public IFormFile ImageFile { get; set;}
    }
}