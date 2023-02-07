namespace BrandexBusinessSuite.SalesAnalysis.Services.Sales;

using System.Collections.Generic;
using System.Threading.Tasks;
using SalesAnalysis.Models.Sales;

public interface ISalesService
{
    Task UploadBulk(IEnumerable<SaleInputModel> sales);
    Task<List<SalesCheckModel>> GetAll();
}