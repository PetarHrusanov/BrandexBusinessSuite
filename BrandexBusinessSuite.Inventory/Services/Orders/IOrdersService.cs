using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Inventory.Models.Orders;

namespace BrandexBusinessSuite.Inventory.Services.Orders;

public interface IOrdersService
{
    Task Upload(OrderInputModel inputModel);
    Task<List<MaterialsQuantitiesOutputModel>> GetLatest();
}