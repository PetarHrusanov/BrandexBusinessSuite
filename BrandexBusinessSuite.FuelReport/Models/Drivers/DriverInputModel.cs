namespace BrandexBusinessSuite.FuelReport.Models.Drivers;

public class DriverInputModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string LastName { get; set; }

    public string UserId { get; set; }
    
    public bool Active { get; set; }
}