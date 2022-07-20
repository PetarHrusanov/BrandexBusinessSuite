namespace BrandexBusinessSuite.OnlineShop.Models;

public class SalesOnlineAnalysisInput
{
    public string OrderNumber { get; set; }
    public DateTime? Date { get; set; }
    public int ProductId { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Total { get; set; }
    public string City { get; set; }
    public string? Sample { get; set; }
    public string? AdSource { get; set; }
}