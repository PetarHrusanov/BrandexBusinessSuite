namespace BrandexBusinessSuite.MarketingAnalysis.Services.MediaTypes;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.MarketingAnalysis.Data;
using BrandexBusinessSuite.MarketingAnalysis.Models.MediaTypes;

public class MediaTypesService : IMediaTypesService
{
    
    private readonly MarketingAnalysisDbContext _db;
    public MediaTypesService(MarketingAnalysisDbContext db) => _db = db;

    public async Task<List<MediaTypesCheckModel>> GetCheckModels() 
        => await _db.MediaTypes.Select(p => new MediaTypesCheckModel() 
        { 
            Id = p.Id, 
            Name = p.Name 
        }).ToListAsync();
}