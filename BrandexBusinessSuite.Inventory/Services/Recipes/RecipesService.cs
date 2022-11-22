using System.Diagnostics;
using BrandexBusinessSuite.Inventory.Data;
using BrandexBusinessSuite.Inventory.Data.Models;
using BrandexBusinessSuite.Inventory.Models.Recipes;
using Microsoft.EntityFrameworkCore;

namespace BrandexBusinessSuite.Inventory.Services.Recipes;

public class RecipesService :IRecipesService
{
    
    private readonly InventoryDbContext _db;

    public RecipesService(InventoryDbContext db)
    {
        _db = db;
    }
    
    public async Task PostRecipe(RecipeInputModel inputModel)
    {
        var recipe = new Recipe
        {
            MaterialId = inputModel.MaterialId,
            ProductId = inputModel.ProductId,
            QuantityRequired = inputModel.QuantityRequired
        };
        
        await _db.Recipes.AddAsync(recipe);
        await _db.SaveChangesAsync();
    }
    
    public async Task EditRecipe(RecipeInputModel inputModel)
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(r =>
            r.ProductId == inputModel.ProductId && r.MaterialId == inputModel.MaterialId);
        recipe!.QuantityRequired = inputModel.QuantityRequired;
        await _db.SaveChangesAsync();
    }

    public async Task<List<RecipeDisplayModel>> GetRecipesDisplay()
        => await _db.Recipes.Select(r => new RecipeDisplayModel()
        {
            MaterialId = r.MaterialId,
            MaterialName = r.Material.Name,
            ProductId = r.ProductId,
            ProductName = r.Product.Name,
            QuantityRequired = r.QuantityRequired
        }).ToListAsync();
}