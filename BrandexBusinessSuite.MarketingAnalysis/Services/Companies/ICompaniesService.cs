namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

public interface ICompaniesService
{
    Task UploadBulk(List<string> medias);
    Task<List<string>> GetCheckModels();
}