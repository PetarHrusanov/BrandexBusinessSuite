using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;

using SalesAnalysis.Models.PharmacyChains;

public interface IPharmacyChainsService
{
    Task UploadBulk(List<string> pharmacyChain);
    Task<string> UploadPharmacyChain(string chainName);
    Task<List<BasicCheckErpModel>> GetPharmacyChainsCheck();
    
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}