using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

using BrandexBusinessSuite.MarketingAnalysis.Models.Companies;

public interface ICompaniesService
{
    Task UploadBulk(List<CompaniesInputModel> medias);
    Task Upload(BasicErpInputModel inputModel);
    Task<List<CompaniesCheckModel>> GetCheckModels();
}