namespace BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;

using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using Data;
using Models.MarketingActivities;

public class MarketingActivitiesService : IMarketingActivitesService
{
    
    private readonly MarketingAnalysisDbContext _db;
    private readonly IMapper _mapper;

    public MarketingActivitiesService(MarketingAnalysisDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task UploadBulk(List<MarketingActivityInputModel> marketingActivities)
    {
        var entities = marketingActivities.Select(activity => new MarketingActivity
        {
            Description = activity.Description,
            Notes = "",
            Date = activity.Date,
            Price = activity.Price,
            ProductId = activity.ProductId,
            Paid = true,
            ErpPublished = true,
            AdMediaId = activity.AdMediaId,
            MediaTypeId = activity.MediaTypeId,
            CreatedOn = DateTime.Now,
            IsDeleted = false
        }).ToList();

        await _db.BulkInsertAsync(entities);
    }

    public async Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(DateTime date)
     => await _db.MarketingActivities.Where(s=>s.Date.Month==date.Month && s.Date.Year == date.Year)
         .Select(n => new MarketingActivityOutputModel
         {
             Id = n.Id,
             Description = n.Description,
             Notes = n.Notes,
             Date = n.Date.ToString("dd-MM-yyyy"),
             Price = n.Price,
             ProductName = n.Product.Name,
             AdMediaName = n.AdMedia.Name,
             AdMediaType = n.MediaType.Name,
             CompanyName = n.AdMedia.Company.Name,
             Paid = n.Paid,
             ErpPublished = n.ErpPublished

         }).ToArrayAsync();

    public async Task UploadMarketingActivity(MarketingActivityInputModel inputModel)
    {
        if (_db.Products.Any(x=> x.Id == inputModel.ProductId) && _db.AdMedias.Any(x=> x.Id == inputModel.AdMediaId)
            && _db.MediaTypes.Any(x=> x.Id == inputModel.MediaTypeId))
        {
            var dbModel = new MarketingActivity()
            {
                ProductId = inputModel.ProductId,
                AdMediaId = inputModel.AdMediaId,
                MediaTypeId = inputModel.MediaTypeId,
                Date = inputModel.Date,
                Description = inputModel.Description,
                Paid = false,
                ErpPublished = false,
                Price = inputModel.Price,
                Notes = ""
            };

            await _db.MarketingActivities.AddAsync(dbModel);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<MarketingActivityEditModel?> GetDetails(int id)
        =>
            await _mapper.ProjectTo<MarketingActivityEditModel>(
                    _db.MarketingActivities
                        .Where(m => m.Id == id))
                .FirstOrDefaultAsync();

    public async Task<MarketingActivityErpModel?> GetDetailsErp(int id) 
        => await _db.MarketingActivities.Where(m => m.Id == id).Select(a => new MarketingActivityErpModel 
        { 
            Description = a.Description, 
            Date = a.Date, 
            ProductName = a.Product.Name, 
            AdMedia = a.AdMedia.Name, 
            MediaType = a.MediaType.NameBg, 
            Price = a.Price, 
            CompanyName = a.AdMedia.Company.Name, 
            CompanyErpId = a.AdMedia.Company.ErpId,
        }).FirstOrDefaultAsync();

    public async Task<MarketingActivityEditModel?> Edit(MarketingActivityEditModel inputModel)
    {
        var activity = new MarketingActivity
        {
            Id = inputModel.Id,
            ProductId = inputModel.ProductId,
            AdMediaId = inputModel.AdMediaId,
            MediaTypeId = inputModel.MediaTypeId,
            Date = inputModel.Date,
            Description = inputModel.Description,
            Paid = inputModel.Paid,
            ErpPublished = inputModel.ErpPublished,
            Price = inputModel.Price,
            Notes = inputModel.Notes
        };
        _db.MarketingActivities.Update(activity);
        await _db.SaveChangesAsync();
        return inputModel;
    }

    public async Task Delete(int id)
    {
        var activity = await _db.MarketingActivities.FirstOrDefaultAsync(m => m.Id == id);
        _db.Remove(activity);
        await _db.SaveChangesAsync();
    }

    public async Task PayMarketingActivity(int id)
    {
        var marketingActivity = await _db.MarketingActivities.Where(m => m.Id == id).FirstOrDefaultAsync();
        marketingActivity!.Paid = !marketingActivity.Paid;
        await _db.SaveChangesAsync();
    }

    public async Task ErpPublishMarketingActivity(int id)
    {
        var marketingActivity = await _db.MarketingActivities.Where(m => m.Id == id).FirstOrDefaultAsync();
        marketingActivity!.ErpPublished = !marketingActivity.ErpPublished;
        await _db.SaveChangesAsync();
    }
    
    public async Task<DateTime> MarketingActivitiesTemplate()
    {
        var date = _db.MarketingActivities.Max(m => m.Date);
        var marketingActivities = await _db.MarketingActivities
            .Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year).Where(m => m.AdMedia.Name == "FACEBOOK" || m.AdMedia.Name == "GOOGLE ADS")
            .ToListAsync();
        
        date = date.AddMonths(1);

        var newActivities = marketingActivities.Select(activity => new MarketingActivity()
        {
            Description = activity.Description,
            Notes = activity.Notes,
            Date = date,
            Price = activity.Price,
            ProductId = activity.ProductId,
            Paid = true,
            ErpPublished = true,
            AdMediaId = activity.AdMediaId,
            MediaTypeId = activity.MediaTypeId
        }).ToList();

        await _db.MarketingActivities.AddRangeAsync(newActivities);
        await _db.SaveChangesAsync();

        return date;
    }
}