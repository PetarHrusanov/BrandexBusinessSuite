namespace BrandexBusinessSuite.SalesBrandex.Models.Sales;

public class SaleDbInputModel
{
    public string ErpId { get; set; }
    public int PharmacyId { get; set; }
    public int ProductId { get; set; }
    public DateTime Date { get; set; }
    public int Count { get; set; }
}