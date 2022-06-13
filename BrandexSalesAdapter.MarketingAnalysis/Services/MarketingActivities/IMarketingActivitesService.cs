using BrandexSalesAdapter.Models;

namespace BrandexSalesAdapter.MarketingAnalysis.Services.MarketingActivities;

using BrandexSalesAdapter.MarketingAnalysis.Models.MarketingActivities;

public interface IMarketingActivitesService
{
    Task UploadBulk(List<MarketingActivityInputModel> marketingActivities);

    Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(DateTime date);

}