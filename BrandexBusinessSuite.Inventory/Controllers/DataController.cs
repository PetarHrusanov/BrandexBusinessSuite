namespace BrandexBusinessSuite.Inventory.Controllers;

using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using BrandexBusinessSuite.Services;
using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.ErpDocuments;
using Models.Products;
using Models.Suppliers;
using Services.Orders;
using Services.Suppliers;

using Models.Materials;
using Services.Materials;
using Services.Products;

using static  Common.Constants;
using static Common.ErpConstants;
using static Requests.RequestsMethods;

public class DataController :ApiController
{
    
    private readonly ErpUserSettings _erpUserSettings;
    private static readonly HttpClient Client = new();
    
    private readonly IProductsService _productsService;
    private readonly IMaterialsService _materialsService;
    private readonly ISuppliersService _suppliersService;

    private const string QueryDate = "General_Products_Products?$top=10000&$filter=Active%20eq%20true";

    public DataController(IOptions<ErpUserSettings> erpUserSettings, IProductsService productsService,
        IMaterialsService materialsService, ISuppliersService suppliersService, IOrdersService ordersService)
    {
        _erpUserSettings = erpUserSettings.Value;
        _productsService = productsService;
        _materialsService = materialsService;
        _suppliersService = suppliersService;
    }
    
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> GetProducts(ProductsInputModel inputModel)
    {
        
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryDate}");
        
        var productsCheck = await _productsService.GetProductsCheck();
        var productsErp = JsonConvert.DeserializeObject<List<ErpProduct>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
        var productNamesArray = inputModel.ProductNames.Split(", ").ToArray();

        var productsErpSelected = (from productErp in productsErp 
            from product in productNamesArray 
            where product.TrimEnd() == productErp.Name.BG.TrimEnd() 
            select productErp).ToList();

        var productsUnique = (from product in productsErpSelected 
            where productsCheck.All(c => !string.Equals(c.ErpId, product.Id, StringComparison.CurrentCultureIgnoreCase)) 
            select product).ToList();
        
        await _productsService.UploadBulk(productsUnique, inputModel.Pills, inputModel.Blisters);

        return Result.Success;
    }
    
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> GetRawMaterials(RawMaterialInputModel rawMaterialInputModel)
    {
        
        var byteArray = Encoding.ASCII.GetBytes($"{_erpUserSettings.User}:{_erpUserSettings.Password}");
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryDate}");
        
        var materialsCheck = await _materialsService.GetAll();
        var materialsErp = JsonConvert.DeserializeObject<List<ErpProduct>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
        var materialNamesArray = rawMaterialInputModel.MaterialsValue.Split(", ").ToArray();

        var materialsErpSelected = (from materialErp in materialsErp 
            from product in materialNamesArray 
            where product == materialErp.Name.BG 
            select materialErp).ToList();

        var materialsUnique = (from materials in materialsErpSelected 
            where materialsCheck.All(c => !string.Equals(c.ErpId, materials.Id, StringComparison.CurrentCultureIgnoreCase)) 
            select materials).ToList();
        
        await _materialsService.UploadBulk(materialsUnique, rawMaterialInputModel.MaterialsType, rawMaterialInputModel.MaterialsMeasure);

        return Result.Success;
    }
    
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}")]
    public async Task<ActionResult> PostSupplier(SupplierInputModel inputModel)
    {
        await _suppliersService.Upload(inputModel);
        return Result.Success;
    }
}