namespace BrandexBusinessSuite.Inventory.Models.Orders;

public class OrderInputModel
{
    public double Quantity { get; set; }
    public double Price { get; set; }
    public bool Delivered { get; set; }
    public int MaterialId { get; set; }
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
}