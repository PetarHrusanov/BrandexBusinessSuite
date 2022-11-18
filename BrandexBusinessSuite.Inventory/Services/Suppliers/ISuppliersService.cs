using BrandexBusinessSuite.Inventory.Models.Suppliers;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.Inventory.Services.Suppliers;

public interface ISuppliersService
{
    Task<List<BasicCheckModel>> GetAll();
    Task Upload(SupplierInputModel supplier);
}