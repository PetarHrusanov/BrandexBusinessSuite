using BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.AdMedias;

using MarketingAnalysis.Models.AdMedias;

public interface IAdMediasService
{
    Task UploadBulk(List<AdMediaInputModel> medias);
    
    Task Upload(BasicCheckModel inputModel);

    Task<List<AdMediaCheckModel>> GetCheckModels();
}