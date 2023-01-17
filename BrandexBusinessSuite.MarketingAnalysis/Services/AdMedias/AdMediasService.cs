namespace BrandexBusinessSuite.MarketingAnalysis.Services.AdMedias;

using AutoMapper;

using System.Data;
using Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.MarketingAnalysis.Data.Models;
using BrandexBusinessSuite.Models.DataModels;
using MarketingAnalysis.Models.AdMedias;

using static Common.MarketingDataConstants;
using static Common.Constants;

public class AdMediasService :IAdMediasService
{
    private readonly MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    
    public AdMediasService(MarketingAnalysisDbContext db, IConfiguration configuration, IMapper mapper)
    {
        _db = db;
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public async Task UploadBulk(List<AdMediaInputModel> medias)
    {
        await using var con = new SqlConnection(_configuration.GetConnectionString(DefaultConnection));
        con.Open();
        using (var bulkCopy = new SqlBulkCopy(con))
        {
            bulkCopy.DestinationTableName = AdMedias;
            var dataTable = new DataTable();
            dataTable.Columns.Add(Name, typeof(string));
            dataTable.Columns.Add(CompanyId, typeof(int));
            dataTable.Columns.Add(CreatedOn, typeof(DateTime));
            dataTable.Columns.Add(IsDeleted, typeof(bool));
            foreach (var media in medias)
            {
                var row = dataTable.NewRow();
                row[Name] = media.Name;
                row[CompanyId] = media.CompanyId;
                row[CreatedOn] = DateTime.Now;
                row[IsDeleted] = false;
                dataTable.Rows.Add(row);
            }
            await bulkCopy.WriteToServerAsync(dataTable);
        }
        con.Close();
    }

    public async Task Upload(BasicCheckModel inputModel)
    {
        await _db.AdMedias.AddAsync(new AdMedia { 
            Name = inputModel.Name.ToUpper().TrimEnd(),
            CompanyId = inputModel.Id
        });
        await _db.SaveChangesAsync();
    }

    public async Task<List<AdMediaDisplayModel>> GetDisplayModels()
        => await _mapper.ProjectTo<AdMediaDisplayModel>(_db.AdMedias).ToListAsync();

    public async Task<AdMediaCheckModel?> GetDetails(int id) 
        => await _mapper.ProjectTo<AdMediaCheckModel>(_db.AdMedias.Where(a=>a.Id==id)).FirstOrDefaultAsync();

    public async Task<List<BasicCheckModel>> GetCheckModels()
    {
        return await _db.AdMedias.Select(p => new BasicCheckModel
        {
            Id = p.Id,
            Name = p.Name,
        }).ToListAsync();
    }

    public async Task<AdMediaCheckModel> Edit(AdMediaCheckModel inputModel)
    {
        var adMedia = new AdMedia
        {
            Id = inputModel.Id,
            Name = inputModel.Name,
            CompanyId = inputModel.CompanyId
        };
        _db.AdMedias.Update(adMedia);
        await _db.SaveChangesAsync();
        return inputModel;
    }

    public async Task Delete(int id)
    {
        var adMedia = await _db.AdMedias.FindAsync(id);
        _db.Remove(adMedia);
        await _db.SaveChangesAsync();
    }
}