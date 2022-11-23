namespace BrandexBusinessSuite.Inventory.Models.Recipes;

public class RecipeDisplayModel
{
    public int MaterialId { get; set; }
    public string MaterialName { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public double QuantityRequired { get; set; }
    
}