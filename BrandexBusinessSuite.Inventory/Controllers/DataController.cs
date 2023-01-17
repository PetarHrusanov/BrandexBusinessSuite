using System.Net.Http.Headers;

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
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> GetProducts(ProductsInputModel inputModel)
    {
        
        var productsCheck = await _productsService.GetProductsCheck();
        var productsErp = await GetProducts();
        var productsErpSelected = productsErp.Where(productErp => inputModel.ProductNames.Split(", ").Contains(productErp.Name.BG.TrimEnd()));
        var productsCheckDic = productsCheck.ToDictionary(p => p.ErpId, StringComparer.OrdinalIgnoreCase);
        var productsUnique = productsErpSelected.Where(p => !productsCheckDic.ContainsKey(p.Id));

        await _productsService.UploadBulk(productsUnique, inputModel.Pills);

        return Result.Success;
    }
    
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> GetRawMaterials(RawMaterialInputModel rawMaterialInputModel)
    {
        var materialsCheck = await _materialsService.GetAll();
        var materialsErp = await GetProducts();
        var materialNamesArray = rawMaterialInputModel.MaterialsValue.Split(", ");
        var materialsErpSelected = materialsErp.Where(m => materialNamesArray.Contains(m.Name.BG)).ToList();
        var materialsUnique = materialsErpSelected.Where(m => materialsCheck.All(c => c.ErpId != m.Id)).ToList();

        await _materialsService.UploadBulk(materialsUnique, rawMaterialInputModel.MaterialsType, rawMaterialInputModel.MaterialsMeasure);

        return Result.Success;
    }
    
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> PostSupplier(SupplierInputModel inputModel)
    {
        await _suppliersService.Upload(inputModel);
        return Result.Success;
    }
    
    private async Task<IEnumerable<ErpProduct>?> GetProducts()
    {
        AuthenticateUserBasicHeader(Client, _erpUserSettings.User, _erpUserSettings.Password);
        var responseContentJObj = await JObjectByUriGetRequest(Client, $"{ErpRequests.BaseUrl}{QueryDate}");
        return JsonConvert.DeserializeObject<IEnumerable<ErpProduct>>(responseContentJObj["value"]?.ToString() ?? throw new InvalidOperationException("No result for the request"));
    }
}