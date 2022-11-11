using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;


public interface IPharmacyChainsService
{
    Task UploadBulk(List<string> pharmacyChain);
    Task UploadBulkFromErp(List<ErpPharmacyCheck> pharmacyChains);
    Task<string> UploadPharmacyChain(string chainName);
    Task<List<BasicCheckErpModel>> GetPharmacyChainsCheck();
    
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}