namespace BrandexSalesAdapter.ExcelLogic.Services.PharmacyCompanies
{
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Models.Companies;

    public interface IPharmacyCompaniesService
    {
        Task<string> UploadCompany(CompanyInputModel company);

        Task<bool> CheckCompanyByName(string companyName);

        Task<int> IdByName(string companyName);
    }
}
