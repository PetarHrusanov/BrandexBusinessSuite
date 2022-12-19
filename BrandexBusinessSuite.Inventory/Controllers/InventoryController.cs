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
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryLots}");
        var batchesList = JsonConvert.DeserializeObject<List<ErpLot>>(responseContentJObj["value"].ToString());
        
        var productsCheck = await _productsService.GetProductsCheck();

        var batchesRequired = productsCheck.SelectMany(product => batchesList!.Where(b => b.Product.Id == product.ErpId)
                .OrderByDescending(p => p.ExpiryDate)
                .ThenByDescending(p => p.ReceiptDate)
                .Take(3))
            .ToList();
        
        responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryBalancesProducts}");
        var currentBalances = JsonConvert.DeserializeObject<List<ErpCurrentBalances>>(responseContentJObj["value"].ToString());
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
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryBalancesMaterials}");
        var currentBalances = JsonConvert.DeserializeObject<List<ErpCurrentBalances>>(responseContentJObj["value"].ToString());
        
        var productsCheck = await _productsService.GetProductsCheck();
        var recipes = await _recipesService.GetRecipesErpIds();

        var productQuantities = new List<ProductMaterialQuantities>();

        foreach (var product in productsCheck)
        {
            var recipesForProduct = recipes.Where(p => p.ProductId == product.Id).ToList();
            productQuantities.AddRange(from recipe in recipesForProduct
                let substanceStock = currentBalances!.Where(p => p.Product.Id == recipe.MaterialErpId)
                    .Select(r => r.QuantityBase.Value)
                    .FirstOrDefault()
                select new ProductMaterialQuantities(product.Name!, recipe.MaterialName)
                {
                    Quantity = recipe.MaterialType switch
                    {
                        MaterialType.Extract or MaterialType.Excipient => substanceStock / (recipe.QuantityRequired * recipe.ProductPills),
                        MaterialType.Blisters => substanceStock / (recipe.QuantityRequired * recipe.ProductBlisters),
                        _ => substanceStock / recipe.QuantityRequired
                    }
                });
        }

        foreach (var quantity in productQuantities)
        {
            if (double.IsPositiveInfinity(quantity.Quantity)) quantity.Quantity = 1000000000;
            if (double.IsNegativeInfinity(quantity.Quantity)) quantity.Quantity = 0;
        }

        return productQuantities;

    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<MaterialsQuantitiesOutputModel>> GetMaterials()
    {
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryBalancesMaterials}");
        var currentBalances = JsonConvert.DeserializeObject<List<ErpCurrentBalances>>(responseContentJObj["value"].ToString());

        var ordersLatest = await _ordersService.GetLatest();
        
        foreach (var order in ordersLatest)
        {
            var stockQuantity = currentBalances!.Where(c => c.Product.Id == order.MaterialErpId)
                .Select(o => o.QuantityBase.Value).FirstOrDefault();
            order.QuantityStock = stockQuantity;
        }

        return ordersLatest;

    }
    
}