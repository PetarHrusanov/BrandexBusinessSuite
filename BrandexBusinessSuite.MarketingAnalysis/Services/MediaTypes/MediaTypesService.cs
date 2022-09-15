namespace BrandexBusinessSuite.MarketingAnalysis.Services.MediaTypes;

using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.MarketingAnalysis.Data;
using BrandexBusinessSuite.MarketingAnalysis.Models.MediaTypes;

public class MediaTypesService : IMediaTypesService
{
    
    private MarketingAnalysisDbContext _db;
    private readonly IConfiguration _configuration;
    
    public MediaTypesService(MarketingAnalysisDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }
    
    public async Task<List<MediaTypesCheckModel>> GetCheckModels()
    {
        return await _db.MediaTypes.Select(p => new MediaTypesCheckModel()
        {
            Id = p.Id,
            Name = p.Name
        }).ToListAsync();
    }
}