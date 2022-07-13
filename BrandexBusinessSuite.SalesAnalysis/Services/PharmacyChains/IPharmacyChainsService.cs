using BrandexBusinessSuite.SalesAnalysis.Models.PharmacyChains;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;

using SalesAnalysis.Models.PharmacyChains;

public interface IPharmacyChainsService
{
    Task UploadBulk(List<string> pharmacyChain);
    Task<string> UploadPharmacyChain(string chainName);

    Task<bool> CheckPharmacyChainByName(string companyName);

    Task<int> IdByName(string companyName);
        
    Task<List<PharmacyChainCheckModel>> GetPharmacyChainsCheck();
}