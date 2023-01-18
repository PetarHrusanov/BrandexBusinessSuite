namespace BrandexBusinessSuite.Inventory.Services.Recipes;

using BrandexBusinessSuite.Inventory.Models.Recipes;

public interface IRecipesService
{
    Task PostRecipe(RecipeInputModel inputModel);

    Task EditRecipe(RecipeInputModel inputModel);
    
    Task Delete(RecipeInputModel inputModel);

    Task<List<RecipeDisplayModel>> GetRecipesDisplay();
    
    Task<List<RecipeErpQuantity>> GetRecipesErpIds();
}