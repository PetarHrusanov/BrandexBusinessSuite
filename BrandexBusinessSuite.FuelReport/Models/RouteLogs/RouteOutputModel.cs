namespace BrandexBusinessSuite.FuelReport.Models.RouteLogs;

public class RouteOutputModel
{
    public string Name { get; set; }
    public string Registration { get; set; }
    public decimal Km { get; set; }
    public decimal MileageStart { get; set; }
    public decimal MileageEnd { get; set; }
    public DateTime Date { get; set; }
}