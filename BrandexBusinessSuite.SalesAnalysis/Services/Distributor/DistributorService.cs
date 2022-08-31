﻿namespace BrandexBusinessSuite.SalesAnalysis.Services.Distributor;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Data;

public class DistributorService :IDistributorService
{
    SalesAnalysisDbContext db;

    public DistributorService(SalesAnalysisDbContext db)
    { 
        this.db = db;
    }

    public async Task<int> IdByName(string input)
    {
        return await db.Distributors.Where(d => d.Name == input).Select(d => d.Id).FirstOrDefaultAsync();
    }
}