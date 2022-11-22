using BrandexBusinessSuite.Inventory.Models.Recipes;

namespace BrandexBusinessSuite.Inventory.Services.Recipes;

public interface IRecipesService
{
    // Task<List<BasicCheckErpModel>> GetProductsCheck();
    // Task UploadBulk(List<ErpProduct> products);

    Task PostRecipe(RecipeInputModel inputModel);

    Task EditRecipe(RecipeInputModel inputModel);

    Task<List<RecipeDisplayModel>> GetRecipesDisplay();
}