namespace BrandexBusinessSuite.Inventory.Models.Materials;

public class MaterialsQuantitiesOutputModel
{
    public string MaterialName { get; set; }
    public string MaterialErpId { get; set; }
    public double QuantityStock { get; set; }
    public string SupplierName { get; set; }
    public string Notes { get; set; }
    public double Price { get; set; }
    public double QuantityOrdered { get; set; }
    public double PriceQuantity { get; set; }
    public string OrderDate { get; set; }
    public string? DeliveryDate { get; set; }
    public bool Delivered { get; set; }

}