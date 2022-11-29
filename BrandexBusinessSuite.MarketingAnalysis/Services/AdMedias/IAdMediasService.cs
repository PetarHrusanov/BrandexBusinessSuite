using BrandexBusinessSuite.MarketingAnalysis.Models.AdMedias;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.AdMedias;

using MarketingAnalysis.Models.AdMedias;

public interface IAdMediasService
{
    Task UploadBulk(List<AdMediaInputModel> medias);

    Task Upload(BasicCheckModel inputModel);
    Task<List<AdMediaDisplayModel>> GetDisplayModels();
    
    Task<AdMediaCheckModel?> GetDetails(int id);

    Task<List<BasicCheckModel>> GetCheckModels();
    
    Task<AdMediaCheckModel> Edit(AdMediaCheckModel inputModel);
    Task Delete(int id);
}