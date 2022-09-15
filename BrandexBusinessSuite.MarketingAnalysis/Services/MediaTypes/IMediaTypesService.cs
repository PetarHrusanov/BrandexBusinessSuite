namespace BrandexBusinessSuite.MarketingAnalysis.Services.MediaTypes;

using BrandexBusinessSuite.MarketingAnalysis.Models.MediaTypes;

public interface IMediaTypesService
{
    Task<List<MediaTypesCheckModel>> GetCheckModels();
}