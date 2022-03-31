namespace BrandexSalesAdapter.ExcelLogic.Services.PharmacyCompanies
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using BrandexSalesAdapter.ExcelLogic.Models.PharmacyCompanies;

    public interface IPharmacyCompaniesService
    {
        Task<string> UploadCompany(PharmacyCompanyInputModel company);

        Task<bool> CheckCompanyByName(string companyName);

        Task<int> IdByName(string companyName);
        
        Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck();
    }
}
