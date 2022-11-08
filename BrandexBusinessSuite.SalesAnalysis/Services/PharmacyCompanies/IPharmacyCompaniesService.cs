using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyCompanies;

using System.Threading.Tasks;
using System.Collections.Generic;
using SalesAnalysis.Models.PharmacyCompanies;

public interface IPharmacyCompaniesService
{
    Task UploadBulk(List<PharmacyCompanyInputModel> pharmacyCompanies);
    Task<string> UploadCompany(PharmacyCompanyInputModel company);
    Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck();
    Task<List<BasicCheckErpModel>> GetPharmacyCompaniesErpCheck();
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}