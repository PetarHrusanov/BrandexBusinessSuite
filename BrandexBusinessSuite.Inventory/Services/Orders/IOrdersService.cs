using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Inventory.Models.Orders;

namespace BrandexBusinessSuite.Inventory.Services.Orders;

public interface IOrdersService
{
    Task Upload(OrderInputModel inputModel);
    
    Task Delete(int id);
    Task<OrderEditModel> GetOrder(int id);
    Task<OrderEditModel> Edit(OrderEditModel inputModel);
    
    Task DeliverOrder(int id);
    
    Task<List<MaterialsQuantitiesOutputModel>> GetLatest();
    
    Task<List<OrderOutputModel>> GetSpecificOrders(int ordersNumber, int? materialId);
    
    Task<List<OrderOutputModel>> GetUndelivered();
}