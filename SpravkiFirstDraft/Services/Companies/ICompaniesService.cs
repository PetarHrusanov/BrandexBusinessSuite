using System;
using System.Threading.Tasks;
using SpravkiFirstDraft.Models.Companies;

namespace SpravkiFirstDraft.Services.Companies
{
    public interface ICompaniesService
    {
        Task<string> UploadCompany(CompanyInputModel company);

        Task<bool> CheckCompanyByName(string companyName);

        Task<int> IdByName(string companyName);
    }
}
