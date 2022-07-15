using BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.AdMedias;

using MarketingAnalysis.Models.AdMedias;

public interface IAdMediasService
{
    Task UploadBulk(List<AdMediaInputModel> medias);

    Task<List<AdMediaCheckModel>> GetCheckModels();
}