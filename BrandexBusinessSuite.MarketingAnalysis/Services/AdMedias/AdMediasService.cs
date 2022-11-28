using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.MarketingAnalysis.Services.AdMedias;

using System.Data;
using Data;
using MarketingAnalysis.Models.AdMedias;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class AdMediasService :IAdMediasService
{
    private MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    
    public AdMediasService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
    public async Task UploadBulk(List<AdMediaInputModel> medias)
    {
        var table = new DataTable();
        table.TableName = AdMedias;
            
        table.Columns.Add(Name, typeof(string));
        table.Columns.Add(CompanyId, typeof(int));
        
        table.Columns.Add(CreatedOn);
        table.Columns.Add(IsDeleted, typeof(bool));
            
        foreach (var media in medias)
        {
            var row = table.NewRow();
            row[Name] = media.Name;
            row[CompanyId] = media.CompanyId;
            
            row[CreatedOn] = DateTime.Now;
            row[IsDeleted] = false;
            
            table.Rows.Add(row);
        }

        var connection = _configuration.GetConnectionString(DefaultConnection);
            
        var con = new SqlConnection(connection);
            
        var objbulk = new SqlBulkCopy(con);  
            
        objbulk.DestinationTableName = AdMedias;
            
        objbulk.ColumnMappings.Add(Name, Name);
        objbulk.ColumnMappings.Add(CompanyId, CompanyId);
        objbulk.ColumnMappings.Add(CreatedOn, CreatedOn);
        objbulk.ColumnMappings.Add(IsDeleted, IsDeleted);

        con.Open();
        await objbulk.WriteToServerAsync(table);  
        con.Close();  
        
    }

    public async Task Upload(BasicCheckModel inputModel)
    {
        var adMedia = new AdMedia()
        {
            Name = inputModel.Name.ToUpper().TrimEnd(),
            CompanyId = inputModel.Id
        };
        await _db.AdMedias.AddAsync(adMedia);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AdMediaCheckModel>> GetCheckModels()
    {
        return await _db.AdMedias.Select(p => new AdMediaCheckModel()
        {
            Id = p.Id,
            Name = p.Name,
            // MediaType = p.MediaType
        }).ToListAsync();
    }
}