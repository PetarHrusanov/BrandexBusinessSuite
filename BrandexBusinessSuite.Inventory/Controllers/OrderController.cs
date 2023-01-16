namespace BrandexBusinessSuite.Inventory.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BrandexBusinessSuite.Controllers;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Services;
using Models.Orders;
using Services.Materials;
using Services.Orders;
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

    [HttpPost]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> DeleteOrder([FromForm] int orderId)
    {
        await _ordersService.Delete(orderId);
        return Result.Success;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    [Route(Id)]
    public async Task<ActionResult<OrderEditModel>> Details(int id)
        => await _ordersService.GetOrder(id) ?? throw new InvalidOperationException();
    
    [HttpPut]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}")]
    public async Task<ActionResult<OrderEditModel>> Edit(OrderEditModel input)
        => await _ordersService.Edit(input) ?? throw new InvalidOperationException();
    
    [HttpPut]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<ActionResult> DeliverOrder([FromForm] int orderId)
    {
        await _ordersService.DeliverOrder(orderId);
        return Result.Success;
    }
    
    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<OrderOutputModel>> GetOrdersUndelivered()
        => await _ordersService.GetOrders(false, Int32.MaxValue, null);


    [HttpGet]
    [Authorize(Roles = $"{AdministratorRoleName}, {AccountantRoleName}, {MarketingRoleName}, {ViewerExecutive}")]
    public async Task<List<OrderOutputModel>> GetSpecificOrders([FromQuery] int ordersNumber,
        [FromQuery] string? materialId)
    {
        var materialConverted = int.TryParse(materialId, out var id) ? id : (int?)null;
        return await _ordersService.GetOrders(true, ordersNumber, materialConverted);
    }

}