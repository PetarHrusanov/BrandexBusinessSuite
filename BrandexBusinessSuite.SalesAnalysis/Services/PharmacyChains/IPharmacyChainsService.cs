using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;


public interface IPharmacyChainsService
{
    Task UploadBulk(List<ErpPharmacyCheck> pharmacyChains);
    Task<List<BasicCheckErpModel>> GetAllCheck();
    
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}