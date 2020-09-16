using System;
using System.Threading.Tasks;
using SpravkiFirstDraft.Models.Companies;

namespace SpravkiFirstDraft.Services.Companies
{
    public interface ICompaniesService
    {
        Task<bool> UploadCompany(CompanyInputModel company);
    }
}
