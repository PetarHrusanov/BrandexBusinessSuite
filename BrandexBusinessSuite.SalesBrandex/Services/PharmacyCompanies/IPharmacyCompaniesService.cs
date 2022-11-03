namespace BrandexBusinessSuite.SalesBrandex.Services.PharmacyCompanies;

using System.Threading.Tasks;
using System.Collections.Generic;

using BrandexBusinessSuite.SalesBrandex.Models.PharmacyCompanies;

public interface IPharmacyCompaniesService
{
    Task UploadBulk(List<PharmacyCompanyInputModel> pharmacyCompanies);
    Task<string> UploadCompany(PharmacyCompanyInputModel company);
    Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck();
}