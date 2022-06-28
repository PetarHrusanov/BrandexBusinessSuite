namespace BrandexBusinessSuite.ExcelLogic.Services.PharmacyCompanies;

using System.Threading.Tasks;
using System.Collections.Generic;
using BrandexBusinessSuite.ExcelLogic.Models.PharmacyCompanies;

public interface IPharmacyCompaniesService
{
    Task UploadBulk(List<PharmacyCompanyInputModel> pharmacyCompanies);
    Task<string> UploadCompany(PharmacyCompanyInputModel company);

    Task<bool> CheckCompanyByName(string companyName);

    Task<int> IdByName(string companyName);
        
    Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck();
}