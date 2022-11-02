namespace BrandexBusinessSuite.SalesBrandex.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.Models;

public interface IPharmacyChainsService
{
    Task UploadBulk(List<string> pharmacyChain);
    Task<string> UploadPharmacyChain(string chainName);
    Task<List<BasicCheckModel>> GetPharmacyChainsCheck();
}