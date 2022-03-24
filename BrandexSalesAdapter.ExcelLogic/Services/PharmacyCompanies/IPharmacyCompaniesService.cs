using BrandexSalesAdapter.ExcelLogic.Models.PharmacyCompanies;

namespace BrandexSalesAdapter.ExcelLogic.Services.PharmacyCompanies
{
    using System.Threading.Tasks;

    public interface IPharmacyCompaniesService
    {
        Task<string> UploadCompany(PharmacyCompanyInputModel company);

        Task<bool> CheckCompanyByName(string companyName);

        Task<int> IdByName(string companyName);
    }
}
