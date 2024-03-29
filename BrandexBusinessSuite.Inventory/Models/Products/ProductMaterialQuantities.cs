namespace BrandexBusinessSuite.Inventory.Models.Products;

public class ProductMaterialQuantities
{
    public ProductMaterialQuantities(string productName, string materialName)
    {
        ProductName = productName;
        MaterialName = materialName;
    }
    public string ProductName { get; set; }
    public string MaterialName { get; set; }
    public double Quantity { get; set; }
    public double QuantityProduct { get; set; }
    public bool Delivered { get; set; }
    public double LastOrderQuantity { get; set; }
}