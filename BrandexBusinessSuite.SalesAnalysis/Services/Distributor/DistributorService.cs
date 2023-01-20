using System.Collections.Generic;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Distributor;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Data;

public class DistributorService :IDistributorService
{
    private readonly SalesAnalysisDbContext _db;
    public DistributorService(SalesAnalysisDbContext db) => _db = db;

    public async Task<int> IdByName(string input) 
        => await _db.Distributors.Where(d => d.Name == input).Select(d => d.Id).FirstOrDefaultAsync();

    public async Task<List<BasicCheckModel>> GetAllCheck()
        => await _db.Distributors.Select(c => new BasicCheckModel
        {
            Name = c.Name,
            Id = c.Id
        }).ToListAsync();
}