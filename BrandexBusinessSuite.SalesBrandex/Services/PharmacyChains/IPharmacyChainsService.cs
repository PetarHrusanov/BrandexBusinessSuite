using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesBrandex.Services.PharmacyChains;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.Models;

public interface IPharmacyChainsService
{
    Task UploadBulk(List<BasicErpInputModel> pharmacyChain);
    Task<string> UploadPharmacyChain(string chainName);
    Task<List<BasicCheckErpModel>> GetPharmacyChainsCheck();
}