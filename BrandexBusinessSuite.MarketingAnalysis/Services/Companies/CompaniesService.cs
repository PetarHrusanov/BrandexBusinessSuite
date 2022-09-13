using BrandexBusinessSuite.MarketingAnalysis.Data;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.Companies;

public class CompaniesService : ICompaniesService
{
    
    private MarketingAnalysisDbContext db;
    private readonly IConfiguration _configuration;
    
    public CompaniesService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        this.db = db;
        _configuration = configuration;
    }
    
    
    public Task UploadBulk(List<string> medias)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetCheckModels()
    {
        throw new NotImplementedException();
    }
}