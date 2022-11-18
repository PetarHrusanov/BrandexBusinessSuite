using BrandexBusinessSuite.Inventory.Models.Orders;

namespace BrandexBusinessSuite.Inventory.Services.Orders;

public interface IOrdersService
{
    Task Upload(OrderInputModel inputModel);
}