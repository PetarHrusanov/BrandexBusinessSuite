namespace BrandexBusinessSuite.SalesAnalysis.Services.PharmacyCompanies;

using System.Threading.Tasks;
using System.Collections.Generic;
using SalesAnalysis.Models.PharmacyCompanies;

using BrandexBusinessSuite.Models.DataModels;
using BrandexBusinessSuite.Models.ErpDocuments;

public interface IPharmacyCompaniesService
{
    Task UploadBulk(List<ErpPharmacyCheck> pharmacyCompanies);
    Task<List<PharmacyCompanyCheckModel>> GetPharmacyCompaniesCheck();
    Task<List<BasicCheckErpModel>> GetAllCheck();
    Task BulkUpdateData(List<BasicCheckErpModel> list);
}