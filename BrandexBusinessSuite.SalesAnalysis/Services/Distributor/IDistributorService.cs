using System.Collections.Generic;
using BrandexBusinessSuite.Models.DataModels;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Distributor;

using System.Threading.Tasks;

public interface IDistributorService
{
    Task<int> IdByName(string input);

    Task<List<BasicCheckModel>> GetAllCheck();
}