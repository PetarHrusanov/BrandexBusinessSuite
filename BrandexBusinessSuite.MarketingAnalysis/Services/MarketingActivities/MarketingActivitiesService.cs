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
    private const string Facebook = "FACEBOOK";
    private const string Google = "GOOGLE ADS";
    
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
            .Include(m => m.AdMedia) // Include the AdMedia entity
            .Where(m => m.Date.Month == date.Month && m.Date.Year == date.Year)
            .ToListAsync();

        if (!complete)
        {
            marketingActivities = marketingActivities
                .Where(m => m.AdMedia.Name == "FACEBOOK" || m.AdMedia.Name == "GOOGLE ADS")
                .ToList();
        }

        // foreach (var t in marketingActivities
        //              .Where(t => t.AdMedia != null &&
        //                          !string.Equals(t.AdMedia.Name, "FACEBOOK", StringComparison.OrdinalIgnoreCase) &&
        //                          !string.Equals(t.AdMedia.Name, "GOOGLE ADS", StringComparison.OrdinalIgnoreCase)))
        // {
        //     t.Paid = false;
        //     t.ErpPublished = false;
        // }
        //
        // date = date.AddMonths(1);
        //
        // var newActivities = marketingActivities.Select(activity => new MarketingActivity()
        // {
        //     Description = activity.Description,
        //     Notes = activity.Notes,
        //     Date = date,
        //     Price = activity.Price,
        //     ProductId = activity.ProductId,
        //     Paid = activity.Paid,
        //     ErpPublished = activity.Paid,
        //     AdMediaId = activity.AdMediaId,
        //     MediaTypeId = activity.MediaTypeId
        // }).ToList();
        
        var modifiedActivities = marketingActivities
            .Select(activity =>
            {
                if (activity.AdMedia != null &&
                    !string.Equals(activity.AdMedia.Name, "FACEBOOK", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(activity.AdMedia.Name, "GOOGLE ADS", StringComparison.OrdinalIgnoreCase))
                {
                    return new MarketingActivity
                    {
                        Description = activity.Description,
                        Notes = activity.Notes,
                        Date = activity.Date,
                        Price = activity.Price,
                        ProductId = activity.ProductId,
                        Paid = false,
                        ErpPublished = false,
                        AdMediaId = activity.AdMediaId,
                        MediaTypeId = activity.MediaTypeId
                    };
                }
                return activity;
            })
            .ToList();

        date = date.AddMonths(1);

        var newActivities = modifiedActivities
            .Select(activity => new MarketingActivity
            {
                Description = activity.Description,
                Notes = activity.Notes,
                Date = date,
                Price = activity.Price,
                ProductId = activity.ProductId,
                Paid = activity.Paid,
                ErpPublished = activity.Paid,
                AdMediaId = activity.AdMediaId,
                MediaTypeId = activity.MediaTypeId
            })
            .ToList();


        await _db.MarketingActivities.AddRangeAsync(newActivities);
        await _db.SaveChangesAsync();

        return date;
    }

    public async Task UploadFacebookAdSets(SocialMediaBudgetInputModel inputModel, decimal euroRate)
    {

        var marketingActivitiesToDelete = await _db.MarketingActivities
            .Where(s => s.Date.Month == inputModel.Date.Month && s.Date.Year == inputModel.Date.Year).Where(a=>a.AdMedia.Name=="FACEBOOK").ToListAsync();
        
        await _db.BulkDeleteAsync(marketingActivitiesToDelete);

        var adMedia = await _db.AdMedias
            .Where(a => a.Name.ToUpper() == "FACEBOOK")
            .Select(a => a.Id)
            .FirstOrDefaultAsync();

        var adMediaType = await _db.MediaTypes
            .Where(a => a.Name.ToUpper() == "FACEBOOK")
            .Select(a => a.Id)
            .FirstOrDefaultAsync();

        await using var stream = new MemoryStream();
        await inputModel.ImageFile.CopyToAsync(stream);

        stream.Position = 0;

        if (!CheckXlsx(inputModel.ImageFile)) throw new Exception("Not xlsx.");

        var hssfwb = new XSSFWorkbook(stream);
        var sheet = hssfwb.GetSheetAt(0);

        var marketingActivitiesUpload = new List<MarketingActivity>();

        for (var i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++) //Read Excel File
        {
            var row = sheet.GetRow(i);
            if (row == null || row.Cells.All(d => d.CellType == CellType.Blank)) continue;
            
            var adSetName = row.GetCell(2).ToString();

            var activity = marketingActivitiesUpload.FirstOrDefault(s => s.Description.Contains(adSetName));
            
            // var price = decimal.Parse(row.GetCell(15).ToString()!) * euroRate;
            
            var priceString = row.GetCell(19).ToString()!;
            priceString = priceString.Replace(',', '.'); // Replace comma with dot
            // var price = decimal.Parse(priceString) * euroRate;

            // var priceString = row.GetCell(15).ToString()!;
            decimal price;
            
            if (decimal.TryParse(priceString, out var parsedPrice))
            {
                price = parsedPrice * euroRate;
                // Process the price further or perform any desired actions
            }
            else
            {
                // Unable to parse the priceString, handle the error or continue with the next iteration
                continue;
            }
            
            var dateString = row.GetCell(0).ToString();
            
            if (activity == null)
            {

                var productId = await _db.Products.Where(s => adSetName.Contains(s.Name)).Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (productId==0)
                {
                    if (adSetName!.Contains("LadyHarmonia"))
                    {
                        productId = await _db.Products.Where(s => s.Name=="Lady Harmonia").Select(s => s.Id)
                            .FirstOrDefaultAsync();
                    }
                    else
                    {
                        productId = await _db.Products.Where(s => s.Name=="Botanic").Select(s => s.Id)
                            .FirstOrDefaultAsync();
                    }
                }

                var marketingActivity = new MarketingActivity()
                {
                    Description = adSetName,
                    Notes = string.Empty,
                    Date = DateTime.Parse(dateString),
                    Price = price,
                    ProductId = productId,
                    Paid = true,
                    ErpPublished = true,
                    AdMediaId = adMedia,
                    MediaTypeId = adMediaType
                };
                
                marketingActivitiesUpload.Add(marketingActivity);
                continue;
            }
            activity.Price +=price;
        }

        await _db.MarketingActivities.AddRangeAsync(marketingActivitiesUpload);
        await _db.SaveChangesAsync();
        
        // await _db.BulkUpdateAsync(marketingActivitiesToChange);
    }

    public async Task UploadGoogleAds(SocialMediaBudgetInputModel inputModel)
    {
        var marketingActivitiesToChange = await _db.MarketingActivities
            .Where(s => s.Date.Month == inputModel.Date.Month && s.Date.Year == inputModel.Date.Year).Where(a=>a.AdMedia.Name=="GOOGLE ADS").ToListAsync();
        
        await _db.BulkDeleteAsync(marketingActivitiesToChange);
        
        var adMedia = await _db.AdMedias
            .Where(a => a.Name.ToUpper() == "GOOGLE ADS")
            .Select(a => a.Id)
            .FirstOrDefaultAsync();

        var adMediaType = await _db.MediaTypes
            .Where(a => a.Name.ToUpper() == "Google")
            .Select(a => a.Id)
            .FirstOrDefaultAsync();
        
        var products = await _db.Products.ToListAsync();
        
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
            
            var type = row.GetCell(1)?.ToString();
            if (type!="Campaigns") continue;

            var adSetName = row.GetCell(2)?.ToString();
            // if (string.IsNullOrWhiteSpace(adSetName) || !marketingActivitiesToChange.Any(activity => activity.Description.Contains(adSetName))) continue;

            if (!activityDictionary.ContainsKey(adSetName))
            {
                activityDictionary.Add(adSetName, 0);
            }

            var priceRow = row.GetCell(4)?.ToString()?.TrimEnd();
            var price = decimal.TryParse(PriceRegex.Match(priceRow)?.Value, out var p) ? p : (decimal?)null;

            if (price.HasValue) activityDictionary[adSetName] += price.Value;
            else activityDictionary[adSetName] = price.GetValueOrDefault();
        }

        var marketingActivitiesForUpload = new List<MarketingActivity>();

        foreach (var entry in activityDictionary)
        {
            // var productId = products.Where(g => entry.Key.Contains(g.Name)).Select(p => p.Id).FirstOrDefault();
            // if (entry.Key.Contains("Display") || entry.Key.Contains("Genral"))
            // {
            //     productId = products.Where(p => p.Name == "Botanic").Select(p => p.Id).FirstOrDefault();
            // }
            
            int productId = products.Where(g => entry.Key.IndexOf(g.Name, StringComparison.OrdinalIgnoreCase) >= 0).Select(g=>g.Id).FirstOrDefault();
            if (entry.Key.IndexOf("Display", StringComparison.OrdinalIgnoreCase) >= 0 || entry.Key.IndexOf("General", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                productId = products.FirstOrDefault(p => string.Equals(p.Name, "Botanic", StringComparison.OrdinalIgnoreCase)).Id;
            }

            var marketingActivity = new MarketingActivity()
            {
                Description = entry.Key,
                Notes = string.Empty,
                Date = inputModel.Date,
                Price = entry.Value,
                ProductId = productId,
                Paid = true,
                ErpPublished = true,
                AdMediaId = adMedia,
                MediaTypeId = adMediaType
            };

            marketingActivitiesForUpload.Add(marketingActivity);
        }
        
        await _db.MarketingActivities.AddRangeAsync(marketingActivitiesForUpload);
        await _db.SaveChangesAsync();

        // foreach (var activity in marketingActivitiesToChange)
        // {
        //     if (activityDictionary.TryGetValue(activity.Description, out var price))
        //     {
        //         activity.Price = price;
        //     }
        // }
        
        // await _db.BulkUpdateAsync(marketingActivitiesToChange);
    }
}