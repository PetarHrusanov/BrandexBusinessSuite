namespace BrandexBusinessSuite.Inventory.Services.Materials;

using BrandexBusinessSuite.Inventory.Models.Materials;
using BrandexBusinessSuite.Models.ErpDocuments;
using Data.Enums;

public interface IMaterialsService
{
    Task<List<MaterialOutputModel>> GetAll();
    Task UploadBulk(List<ErpProduct> products, MaterialType materialType, MaterialMeasurement materialMeasurement);
}