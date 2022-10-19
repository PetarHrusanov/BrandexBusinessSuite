namespace BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;

using MarketingAnalysis.Models.MarketingActivities;

public interface IMarketingActivitesService
{
    Task UploadBulk(List<MarketingActivityInputModel> marketingActivities);

    Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(DateTime date);

    Task UploadMarketingActivity(MarketingActivityInputModel inputModel);

    Task<MarketingActivityEditModel?> GetDetails(int id);
    
    Task<MarketingActivityErpModel?> GetDetailsErp(int id);

    Task<MarketingActivityEditModel?> Edit(MarketingActivityEditModel inputModel);
    
    Task Delete(int id);
    
    Task PayMarketingActivity(int id);
    
    Task ErpPublishMarketingActivity(int id);

    Task<DateTime> MarketingActivitiesTemplate();


}