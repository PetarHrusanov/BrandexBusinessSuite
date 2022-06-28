namespace BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;

using MarketingAnalysis.Models.MarketingActivities;

public interface IMarketingActivitesService
{
    Task UploadBulk(List<MarketingActivityInputModel> marketingActivities);

    Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(DateTime date);

}