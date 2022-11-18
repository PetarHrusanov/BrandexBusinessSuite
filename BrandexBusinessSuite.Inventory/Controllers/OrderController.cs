using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Inventory.Models.Orders;
using BrandexBusinessSuite.Inventory.Services.Materials;
using BrandexBusinessSuite.Inventory.Services.Orders;
using BrandexBusinessSuite.Inventory.Services.Products;
using BrandexBusinessSuite.Inventory.Services.Suppliers;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BrandexBusinessSuite.Inventory.Controllers;

using static  Common.Constants;
using static Common.ErpConstants;
using static Requests.RequestsMethods;

public class OrderController : ApiController
{
    
    private readonly IProductsService _productsService;
    private readonly IMaterialsService _materialsService;
    private readonly ISuppliersService _suppliersService;
    private readonly IOrdersService _ordersService;

    public OrderController(IProductsService productsService, IMaterialsService materialsService,
        ISuppliersService suppliersService, IOrdersService ordersService)
    {
        _productsService = productsService;
        _materialsService = materialsService;
        _suppliersService = suppliersService;
        _ordersService = ordersService;
    }

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<BasicCheckModel>> GetMaterials()
    {
        var materials = await _materialsService.GetAll();
        var materialCheck = materials.Select(m => new BasicCheckModel()
        {
            Name = m.Name,
            Id = m.Id
        }).ToList();
        return materialCheck;
    }

    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<BasicCheckModel>> GetSuppliers()
        => await _suppliersService.GetAll();

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> PostOrder(OrderInputModel inputModel)
    {
        await _ordersService.Upload(inputModel);
        return Result.Success;
    }
}