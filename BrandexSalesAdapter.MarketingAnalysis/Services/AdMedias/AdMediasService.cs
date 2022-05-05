using BrandexSalesAdapter.MarketingAnalysis.Data;
using BrandexSalesAdapter.MarketingAnalysis.Models.AdMedias;

namespace BrandexSalesAdapter.MarketingAnalysis.Services.AdMedias;

public class AdMediasService :IAdMediasService
{
    private MarketingAnalysisDbContext db;
    private readonly IConfiguration _configuration;
    
    
    public AdMediasService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        this.db = db;
        _configuration = configuration;
    }
    
    public Task UploadBulk(List<AdMediaInputModel> medias)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadSingle(string media)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckByName(string mediaName)
    {
        throw new NotImplementedException();
    }

    public Task<int> IdByName(string mediaName)
    {
        throw new NotImplementedException();
    }

    public Task<List<AdMediaCheckModel>> GetCheckModels()
    {
        throw new NotImplementedException();
    }
}