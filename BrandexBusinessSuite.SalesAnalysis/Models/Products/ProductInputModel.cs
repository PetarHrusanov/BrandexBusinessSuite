namespace BrandexBusinessSuite.SalesAnalysis.Models.Products;

public class ProductInputModel
{
    public string Name { get; set; }

    public string ShortName { get; set; }

    public int BrandexId { get; set; }

    public int? PhoenixId { get; set; }

    public int? PharmnetId { get; set; }

    public int? StingId { get; set; }

    public string SopharmaId { get; set; }

    public double Price { get; set; }
}