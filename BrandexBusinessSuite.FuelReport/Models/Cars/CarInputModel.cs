namespace BrandexBusinessSuite.FuelReport.Models.Cars;

public class CarInputModel
{
    public string Registration { get; set; }
    
    public decimal Mileage { get; set; }
    
    public bool Active { get; set; }
    
    public int CarModelId { get; set; }
}