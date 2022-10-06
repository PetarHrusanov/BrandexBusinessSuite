namespace BrandexBusinessSuite.MarketingAnalysis.Services.MarketingActivities;

using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using AutoMapper;

using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using Data;
using Models.MarketingActivities;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class MarketingActivitiesService : IMarketingActivitesService
{
    
    private readonly MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public MarketingActivitiesService(MarketingAnalysisDbContext db, IConfiguration configuration, IMapper mapper)
    {
        _db = db;
        _configuration = configuration;
        _mapper = mapper;

    }

    public async Task UploadBulk(List<MarketingActivityInputModel> marketingActivities)
    {
        var table = new DataTable();
        table.TableName = MarketingActivities;
            
        table.Columns.Add(Description, typeof(string));
        table.Columns.Add(Notes, typeof(string));
        table.Columns.Add(Date, typeof(DateTime));
        table.Columns.Add(Price, typeof(decimal));
        table.Columns.Add(ProductId);
        table.Columns.Add(Paid, typeof(bool));
        table.Columns.Add(ErpPublished, typeof(bool));
        table.Columns.Add(AdMediaId);
        table.Columns.Add(MediaTypeId);
        
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var activity in marketingActivities)
        {
            var row = table.NewRow();
            row[Description] = activity.Description;
            row[Notes] = "";
            row[Date] = activity.Date;
            row[Price] = activity.Price;
            row[ProductId] = activity.ProductId;
            row[Paid] = true;
            row[ErpPublished] = true;
            row[AdMediaId] = activity.AdMediaId;
            row[MediaTypeId] = activity.MediaTypeId;

            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString(DefaultConnection);
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = MarketingActivities;
            
        objbulk.ColumnMappings.Add(Description, Description);
        objbulk.ColumnMappings.Add(Notes, Notes);
        objbulk.ColumnMappings.Add(Date, Date);
        objbulk.ColumnMappings.Add(Price, Price);
        objbulk.ColumnMappings.Add(ProductId, ProductId);
        objbulk.ColumnMappings.Add(Paid, Paid);
        objbulk.ColumnMappings.Add(ErpPublished, ErpPublished);
        objbulk.ColumnMappings.Add(AdMediaId, AdMediaId);
        objbulk.ColumnMappings.Add(MediaTypeId, MediaTypeId);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();
    }

    public async Task<MarketingActivityOutputModel[]> GetMarketingActivitiesByDate(DateTime date)
    {
        return await _db.MarketingActivities.Where(s=>s.Date.Month==date.Month && s.Date.Year == date.Year)
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
    }

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
        // return BadRequest(Result.Failure("Category does not exist.")); 
    }

    public async Task<MarketingActivityEditModel?> GetDetails(int id)
        =>
            await _mapper.ProjectTo<MarketingActivityEditModel>(
                    _db.MarketingActivities
                        .Where(m => m.Id == id))
                .FirstOrDefaultAsync();

    public async Task<MarketingActivityErpModel?> GetDetailsErp(int id)
    {
        return await _db.MarketingActivities.Where(m => m.Id == id).Select(a => new MarketingActivityErpModel
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
    }

    public async Task<MarketingActivityEditModel?> Edit(MarketingActivityEditModel inputModel)
    {
        var activity = await _db.MarketingActivities.Where(m => m.Id == inputModel.Id).FirstOrDefaultAsync();
        activity.ProductId = inputModel.ProductId;
        activity.AdMediaId = inputModel.AdMediaId;
        activity.MediaTypeId = inputModel.MediaTypeId;
        activity.Date = inputModel.Date;
        activity.Description = inputModel.Description;
        activity.Paid = inputModel.Paid;
        activity.ErpPublished = inputModel.ErpPublished;
        activity.Price = inputModel.Price;
        activity.Notes = inputModel.Notes;

        await _db.SaveChangesAsync();
        return inputModel;

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

        foreach (var newActivity in marketingActivities.Select(activity => new MarketingActivity()
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
                 }))
        {
            await _db.MarketingActivities.AddAsync(newActivity);
            await _db.SaveChangesAsync();
        }

        return date;
    }

    // await mapper
            // .ProjectTo<MarketingActivityInputModel>( this .All()
            //     .Where(c => c.Id == id))
            // .FirstOrDefaultAsync();
    
    // private IQueryable<MarketingActivityInputModel> AllAvailable()
    //     => this
    //         .All()
    //         .Where(car => car.IsAvailable);
}