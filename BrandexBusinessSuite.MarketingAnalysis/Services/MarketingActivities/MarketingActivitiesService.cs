using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;

using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using Models.Facebook;
using Data;
using Models.MarketingActivities;

using static Methods.ExcelMethods;

public class MarketingActivitiesService : IMarketingActivitesService
{
    
    private readonly MarketingAnalysisDbContext _db;
    private readonly IMapper _mapper;
    
    private static readonly Regex PriceRegex = new(@"[0-9]+[.,][0-9]*");

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

    public async Task<DateTime> MarketingActivitiesTemplate(bool complete)
    {
        var date = _db.MarketingActivities.Max(m => m.Date);
        var marketingActivities = await _db.MarketingActivities
            .Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year)
            .ToListAsync();

        if (complete is false)
        {
            marketingActivities = marketingActivities
                .Where(m => m.AdMedia.Name == "FACEBOOK" || m.AdMedia.Name == "GOOGLE ADS").ToList();
        }
        
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

    public async Task UploadFacebookAdSets(FileAndDateInputModel inputModel, decimal euroRate)
    {

        var marketingActivitiesToChange = await _db.MarketingActivities
            .Where(s => s.Date.Month == inputModel.Date.Month && s.Date.Year == inputModel.Date.Year).Where(a=>a.AdMedia.Name=="FACEBOOK").ToListAsync();

        await using var stream = new MemoryStream();
        await inputModel.ImageFile.CopyToAsync(stream);

        stream.Position = 0;

        if (!CheckXlsx(inputModel.ImageFile)) throw new Exception("Not xlsx.");

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);
            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;
            
            var adSetName = row.GetCell(2).ToString();

            var activity = marketingActivitiesToChange.FirstOrDefault(s => s.Description.Contains(adSetName));
            if (activity == null) continue;
            var price = decimal.Parse(row.GetCell(15).ToString()!);
            activity.Price = price * euroRate;
        }
        
        await _db.BulkUpdateAsync(marketingActivitiesToChange);
    }

    public async Task UploadGoogleAds(FileAndDateInputModel inputModel)
    {
        var marketingActivitiesToChange = await _db.MarketingActivities
            .Where(s => s.Date.Month == inputModel.Date.Month && s.Date.Year == inputModel.Date.Year).Where(a=>a.AdMedia.Name=="GOOGLE").ToListAsync();

        await using var stream = new MemoryStream();
        await inputModel.ImageFile.CopyToAsync(stream);

        stream.Position = 0;

        if (!CheckXlsx(inputModel.ImageFile)) throw new Exception("Not xlsx.");

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        var activityDictionary = new Dictionary<string, decimal>();

        foreach (IRow row in sheet)
        {
            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;

            var adSetName = row.GetCell(2)?.ToString();
            if (string.IsNullOrWhiteSpace(adSetName) || !marketingActivitiesToChange.Any(activity => activity.Description.Contains(adSetName))) continue;

            if (!activityDictionary.ContainsKey(adSetName))
            {
                activityDictionary.Add(adSetName, 0);
            }

            var priceRow = row.GetCell(4)?.ToString()?.TrimEnd();
            var price = decimal.TryParse(PriceRegex.Match(priceRow)?.Value, out var p) ? p : (decimal?)null;

            if (price.HasValue) activityDictionary[adSetName] += price.Value;
            else activityDictionary[adSetName] = price.GetValueOrDefault();
        }

        foreach (var activity in marketingActivitiesToChange)
        {
            if (activityDictionary.TryGetValue(activity.Description, out var price))
            {
                activity.Price = price;
            }
        }
        
        await _db.BulkUpdateAsync(marketingActivitiesToChange);
    }
}