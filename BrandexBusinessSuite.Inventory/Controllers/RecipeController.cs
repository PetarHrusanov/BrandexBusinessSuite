using BrandexBusinessSuite.Inventory.Services.Materials;
using BrandexBusinessSuite.Inventory.Services.Products;
using BrandexBusinessSuite.Inventory.Services.Recipes;
using BrandexBusinessSuite.Inventory.Services.Suppliers;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.Inventory.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Models.Recipes;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Services;

using static  Common.Constants;


public class RecipeController :ApiController
{
    
    private readonly IProductsService _productsService;
    private readonly IMaterialsService _materialsService;
    private readonly IRecipesService _recipesService;

    public RecipeController(IProductsService productsService, IMaterialsService materialsService, IRecipesService recipesService)
    {
        _productsService = productsService;
        _materialsService = materialsService;
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