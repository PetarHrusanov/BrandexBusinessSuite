namespace BrandexBusinessSuite.Inventory.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Services;
using Models.Orders;
using Services.Materials;
using Services.Orders;
using Services.Products;
using Services.Suppliers;

using static  Common.Constants;

public class OrderController : ApiController
{
    
    private readonly IMaterialsService _materialsService;
    private readonly ISuppliersService _suppliersService;
    private readonly IOrdersService _ordersService;

    public OrderController(IMaterialsService materialsService, ISuppliersService suppliersService, IOrdersService ordersService)
    {
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