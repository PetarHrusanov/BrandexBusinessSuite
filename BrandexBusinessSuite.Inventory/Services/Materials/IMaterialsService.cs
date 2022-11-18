using BrandexBusinessSuite.Inventory.Data.Enums;
using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.Inventory.Services.Materials;

public interface IMaterialsService
{
    Task<List<MaterialOutputModel>> GetAll();
    
    Task UploadBulk(List<ErpProduct> products, MaterialType materialType, MaterialMeasurement materialMeasurement);
}