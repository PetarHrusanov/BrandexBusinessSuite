namespace BrandexSalesAdapter.ExcelLogic.Services.Companies
{
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Models.Companies;

    public interface ICompaniesService
    {
        Task<string> UploadCompany(CompanyInputModel company);

        Task<bool> CheckCompanyByName(string companyName);

        Task<int> IdByName(string companyName);
    }
}
