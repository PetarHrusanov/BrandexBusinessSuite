namespace BrandexBusinessSuite.SalesAnalysis.Services.Sales;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesAnalysis.Models.Sales;

public interface ISalesService
{
    Task UploadBulk(List<SaleInputModel> sales);

    Task<int> ProductCountSumByIdDate(int productId, DateTime? dateBegin, DateTime? dateEnd, int? regionId);

    Task<List<DateTime>> GetDistinctDatesByMonths();
}