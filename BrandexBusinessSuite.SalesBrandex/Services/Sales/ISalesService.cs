using BrandexBusinessSuite.Models;
using BrandexBusinessSuite.Models.Dates;

namespace BrandexBusinessSuite.SalesBrandex.Services.Sales;

using System.Collections.Generic;
using System.Threading.Tasks;

using BrandexBusinessSuite.SalesBrandex.Models.Sales;

public interface ISalesService
{
    Task UploadBulk(List<SaleDbInputModel> sales);

    Task<List<string>> QuickCheckListErpIdByDates(DateStartEndInputModel dateStartEndInputModel);
    
    Task<List<ProductQuantitiesOutputModel>> AverageSales();

    // Task<int> ProductCountSumByIdDate(int productId, DateTime? dateBegin, DateTime? dateEnd, int? regionId);
    //
    // Task<List<DateTime>> GetDistinctDatesByMonths();
}