namespace BrandexBusinessSuite.Inventory.Models.Recipes;

public class RecipeInputModel
{
    public int ProductId { get; set; }
    public int MaterialId { get; set; }
    public double QuantityRequired { get; set; }
}