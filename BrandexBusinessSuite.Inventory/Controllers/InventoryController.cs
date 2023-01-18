using BrandexBusinessSuite.Inventory.Models.Recipes;

namespace BrandexBusinessSuite.Inventory.Controllers;

using System.Net.Http.Headers;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.Models.ErpDocuments;
using Data.Enums;
using Models.Materials;
using Models.Products;
using Services.Orders;
using Services.Products;
using Services.Recipes;

using static Common.Constants;
using static Common.ErpConstants;
using static Requests.RequestsMethods;


public class InventoryController : ApiController
{
    
    private readonly ErpUserSettings _erpUserSettings;
    private static readonly HttpClient Client = new();
    
    private readonly IProductsService _productsService;
    private readonly IRecipesService _recipesService;
    private readonly IOrdersService _ordersService;
    
    private const string QueryLots = "Logistics_Inventory_Lots?$top=100000&$select=ExpiryDate,Id,Number,ReceiptDate&$expand=Product($select=Id)";
    private const string QueryBalancesProducts =
        "Logistics_Inventory_CurrentBalances?$top=1000000&$filter=Store%20eq%20'Logistics_Inventory_Stores(100447ff-44f4-4799-a4c2-7c9b22fb0aaa)'&$select=QuantityBase&$expand=Lot($select=Id),Product($select=Id)";
    private const string QueryBalancesMaterials =
        "Logistics_Inventory_CurrentBalances?$top=1000000&$filter=Store%20eq%20'Logistics_Inventory_Stores(38f4c900-8428-4f94-bc6a-76159b53fb3f)'&$select=QuantityBase&$expand=Lot($select=Id),Product($select=Id,Name)";

    public InventoryController(IOptions<ErpUserSettings> erpUserSettings, IProductsService productsService,
        IOrdersService ordersService, IRecipesService recipesService)
    {
        _erpUserSettings = erpUserSettings.Value;
        _productsService = productsService;
        _ordersService = ordersService;
        _recipesService = recipesService;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<ProductQuantitiesOutputModel>> GetProducts()
    {
        AuthenticateUserBasicHeader(Client, _erpUserSettings.User, _erpUserSettings.Password);

        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryLots}");
        var batchesList = JsonConvert.DeserializeObject<List<ErpLot>>(responseContentJObj["value"].ToString());
        
        var productsCheck = await _productsService.GetProductsCheck();

        var batchesRequired = productsCheck.SelectMany(product => batchesList!.Where(b => b.Product.Id == product.ErpId)
                .OrderByDescending(p => p.ExpiryDate)
                .ThenByDescending(p => p.ReceiptDate)
                .Take(3))
            .ToList();
        
        var currentBalances = await GetCurrentBalances(false);
        var currentBalancesNecessary = batchesRequired
            .Select(batch => currentBalances!.FirstOrDefault(b => b.Lot?.Id == batch.Id)).ToList();

        var productQuantities = (from product in productsCheck
            let sum = (int)currentBalancesNecessary.Where(b => b.Product.Id == product.ErpId)
                .Select(q => q.QuantityBase.Value)
                .Sum()
            select new ProductQuantitiesOutputModel { Quantity = sum, Name = product.Name, ErpId = product.ErpId}).ToList();

        return productQuantities;
    }

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<ProductMaterialQuantities>> GetProductMaterials()
    {
        var currentBalancesDic = (await GetCurrentBalances(true)).ToDictionary(x=>x.Product.Id);
        var productsCheck = await _productsService.GetProductsCheck();
        var recipes = await _recipesService.GetRecipesErpIds();
        var ordersDic = (await _ordersService.GetLatest()).ToDictionary(x=>x.MaterialErpId);

        var productQuantities = (from product in productsCheck
            let recipesForProduct = recipes.Where(p => p.ProductId == product.Id)
            from recipe in recipesForProduct
            let substanceStock = currentBalancesDic.TryGetValue(recipe.MaterialErpId, out var value) ? value.QuantityBase.Value : 0
            let orderLast = ordersDic.TryGetValue(recipe.MaterialErpId, out var order) ? order : null
            select new ProductMaterialQuantities(product.Name!, recipe.MaterialName)
            {
                Quantity = substanceStock,
                QuantityProduct = recipe.MaterialType switch
                {
                    MaterialType.Extract or MaterialType.Excipient => substanceStock / (recipe.QuantityRequired * recipe.ProductPills),
                    _ => substanceStock / recipe.QuantityRequired
                },
                Delivered = orderLast?.Delivered ?? false,
                LastOrderQuantity = orderLast?.QuantityOrdered ?? 0
            }).ToList();

        return productQuantities.Select(quantity => {
            quantity.Quantity = Math.Min(1000000000,quantity.Quantity);
            quantity.Quantity = Math.Max(0,quantity.Quantity);
            return quantity;
        }).ToList();
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<RecipeCalculationModel>> Calculator([FromQuery]int productId, [FromQuery]double quantityForProduction)
    {
        var currentBalancesDic = (await GetCurrentBalances(true)).ToDictionary(x=>x.Product.Id);
        var recipes = (await _recipesService.GetRecipesErpIds()).Where(p => p.ProductId == productId).ToList();
        
        var recipeQuantities = recipes.Select(recipe => new RecipeCalculationModel 
        { 
            MaterialName = recipe.MaterialName, 
            AvailableQuantity = currentBalancesDic.TryGetValue(recipe.MaterialErpId, out var value) ? value.QuantityBase.Value : 0, 
            NecessaryQuantity = recipe.MaterialType switch 
            {
                MaterialType.Extract or MaterialType.Excipient => (recipe.QuantityRequired * recipe.ProductPills) * quantityForProduction,
                _ => recipe.QuantityRequired * quantityForProduction
            },
        }).ToList();
        return recipeQuantities;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<MaterialsQuantitiesOutputModel>> GetMaterials()
    {
        var updatedOrders = 
            from order in await _ordersService.GetLatest()
            join balance in await GetCurrentBalances(true) on order.MaterialErpId equals balance.Product.Id into balances
            from b in balances.DefaultIfEmpty()
            select new {order, QuantityStock = b?.QuantityBase.Value ?? 0};

        return updatedOrders.Select(x=> { x.order.QuantityStock = x.QuantityStock; return x.order;}).ToList();
    }

    private async Task<IEnumerable<ErpCurrentBalances>> GetCurrentBalances(bool materialsOnly)
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}"); 
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray)); 

        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{(materialsOnly ? QueryBalancesMaterials : QueryBalancesProducts)}"); 
        return JsonConvert.DeserializeObject<IEnumerable<ErpCurrentBalances>>(responseContentJObj["value"]!.ToString())!; 
    }
}