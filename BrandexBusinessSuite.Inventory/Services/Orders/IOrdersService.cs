namespace BrandexBusinessSuite.Inventory.Services.Orders;

using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Inventory.Models.Orders;

public interface IOrdersService
{
    Task Upload(OrderInputModel inputModel);
    Task Delete(int id);
    Task<OrderEditModel> GetOrder(int id);
    Task<OrderEditModel> Edit(OrderEditModel inputModel);
    Task DeliverOrder(int id);
    Task<List<MaterialsQuantitiesOutputModel>> GetLatest();
    Task<List<OrderOutputModel>> GetOrders(bool delivered, int ordersNumber, int? materialId);
    
}