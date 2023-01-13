using BrandexBusinessSuite.Inventory.Data.Enums;

namespace BrandexBusinessSuite.Inventory.Models.Recipes;

public class RecipeErpQuantity
{
    public int ProductId { get; set; }
    public int ProductPills { get; set; }
    // public int ProductBlisters { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; }
    public MaterialType MaterialType { get; set; }
    public string MaterialErpId { get; set; }
    public double QuantityRequired { get; set; }
}