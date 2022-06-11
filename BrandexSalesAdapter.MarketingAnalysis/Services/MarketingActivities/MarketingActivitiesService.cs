using BrandexSalesAdapter.Models;
using Microsoft.EntityFrameworkCore;

namespace BrandexSalesAdapter.MarketingAnalysis.Services.MarketingActivities;

using System.Data;

using Microsoft.Data.SqlClient;

using Data;
using Models.MarketingActivities;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class MarketingActivitiesService :IMarketingActivitesService
{
    
    MarketingAnalysisDbContext db;
    private readonly IConfiguration _configuration;

    public MarketingActivitiesService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        this.db = db;
        _configuration = configuration;

    }
    
    public async Task UploadBulk(List<MarketingActivityInputModel> marketingActivities)
    {
        var table = new DataTable();
        table.TableName = MarketingActivities;
            
        table.Columns.Add(Description, typeof(string));
        table.Columns.Add(Date, typeof(DateTime));
        table.Columns.Add(Price, typeof(decimal));
        table.Columns.Add(ProductId);
        table.Columns.Add(AdMediaId);
        
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var activity in marketingActivities)
        {
            var row = table.NewRow();
            row[Description] = activity.Description;
            row[Date] = activity.Date;
            row[Price] = activity.Price;
            row[ProductId] = activity.ProductId;
            row[AdMediaId] = activity.AdMediaId;

            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        string connection = _configuration.GetConnectionString(DefaultConnection);
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = MarketingActivities;
            
        objbulk.ColumnMappings.Add(Description, Description);
        objbulk.ColumnMappings.Add(Date, Date);
        objbulk.ColumnMappings.Add(Price, Price);
        objbulk.ColumnMappings.Add(ProductId, ProductId);
        objbulk.ColumnMappings.Add(AdMediaId, AdMediaId);
        
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();
    }

    public async Task<MarketingActivityModel[]> GetMarketingActivitiesByDate(DateTime date)
    {
        return await db.MarketingActivities.Where(s=>s.Date.Month==date.Month && s.Date.Year == date.Year)
            .Select(n => new MarketingActivityModel
        {
            Id = n.Id,
            Description = n.Description,
            Date = n.Date,
            Price = n.Price,
            ProductId = n.ProductId,
            AdMediaId = n.AdMediaId
            
        }).ToArrayAsync();
    }
}