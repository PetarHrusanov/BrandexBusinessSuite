namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

public interface IPharmacyChainsService
{
    Task UploadBulk(List<ErpPharmacyCheck> pharmacyChains);
    Task<List<BasicCheckErpModel>> GetAllCheck();
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}