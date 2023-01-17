using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

using BrandexBusinessSuite.MarketingAnalysis.Models.Companies;

public interface ICompaniesService
{
    Task UploadBulk(List<CompaniesInputModel> companies);
    Task Upload(BasicErpInputModel inputModel);
    Task<List<BasicCheckModel>> GetCheckModels();
    Task<BasicCheckErpModel> Details(int id);
    Task<BasicCheckErpModel> Edit(BasicCheckErpModel inputModel);
    Task Delete(int id);
}