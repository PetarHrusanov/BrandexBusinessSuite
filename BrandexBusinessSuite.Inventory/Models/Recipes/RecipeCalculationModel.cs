namespace BrandexBusinessSuite.Inventory.Models.Recipes;

public class RecipeCalculationModel
{
    public string MaterialName { get; set; }
    public double NecessaryQuantity { get; set; }
    public double AvailableQuantity { get; set; }
}