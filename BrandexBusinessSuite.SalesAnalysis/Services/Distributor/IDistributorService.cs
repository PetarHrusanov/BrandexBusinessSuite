namespace BrandexBusinessSuite.SalesAnalysis.Services.Distributor;

using System.Threading.Tasks;

public interface IDistributorService
{
    Task<bool> CheckDistributor(string input);

    Task<int> IdByName(string input);
}