namespace BrandexBusinessSuite.Inventory.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Services;
using Models.Recipes;
using Services.Products;
using Services.Recipes;

using static  Common.Constants;

public class RecipeController :ApiController
{
    
    private readonly IProductsService _productsService;
    private readonly IRecipesService _recipesService;

    public RecipeController(IProductsService productsService, IRecipesService recipesService)
    {
        _productsService = productsService;
        _recipesService = recipesService;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<BasicCheckModel>> GetProducts()
    {
        var products = await _productsService.GetProductsCheck();
        return products.Select(m => new BasicCheckModel()
        { 
            Name = m.Name,
            Id = m.Id
        }).ToList();
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<RecipeDisplayModel>> GetRecipesDisplay() 
        => await _recipesService.GetRecipesDisplay();

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> PostRecipe(RecipeInputModel inputModel)
    {
        await _recipesService.PostRecipe(inputModel);
        return Result.Success;
    }
    
    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> EditRecipe(RecipeInputModel inputModel)
    {
        await _recipesService.EditRecipe(inputModel);
        return Result.Success;
    }
}