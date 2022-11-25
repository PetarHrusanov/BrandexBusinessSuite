namespace BrandexBusinessSuite.Inventory.Models.Orders;

public class OrderInputModel
{
    public double Quantity { get; set; }
    public double Price { get; set; }
    public string? Notes { get; set; }
    public int MaterialId { get; set; }
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
}