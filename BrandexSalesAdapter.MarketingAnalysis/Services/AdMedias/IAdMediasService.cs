namespace BrandexSalesAdapter.MarketingAnalysis.Services.AdMedias;

using BrandexSalesAdapter.MarketingAnalysis.Models.AdMedias;

public interface IAdMediasService
{
    Task UploadBulk(List<AdMediaInputModel> medias);
        
    Task<string> UploadSingle(string media);

    Task<bool> CheckByName(string mediaName);

    Task<int> IdByName(string mediaName);

    Task<List<AdMediaCheckModel>> GetCheckModels();
}