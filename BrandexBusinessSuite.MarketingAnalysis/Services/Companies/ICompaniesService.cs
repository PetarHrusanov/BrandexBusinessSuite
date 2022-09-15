namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

using BrandexBusinessSuite.MarketingAnalysis.Models.Companies;

public interface ICompaniesService
{
    Task UploadBulk(List<string> medias);
    Task<List<CompaniesCheckModel>> GetCheckModels();
}