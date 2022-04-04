namespace BrandexSalesAdapter.ExcelLogic.Services.PharmacyChains
{
    using System;
    using System.Threading.Tasks;
    
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Models.PharmacyChains;

    public interface IPharmacyChainsService
    {
        Task UploadBulk(List<string> pharmacyChain);
        Task<string> UploadPharmacyChain(string chainName);

        Task<bool> CheckPharmacyChainByName(string companyName);

        Task<int> IdByName(string companyName);
        
        Task<List<PharmacyChainCheckModel>> GetPharmacyChainsCheck();
    }
}
