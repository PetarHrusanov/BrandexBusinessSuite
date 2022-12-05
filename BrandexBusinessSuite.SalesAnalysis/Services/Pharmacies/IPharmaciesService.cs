namespace BrandexBusinessSuite.SalesAnalysis.Services.Pharmacies;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesAnalysis.Models.Pharmacies;

using BrandexBusinessSuite.Models.DataModels;

public interface IPharmaciesService
{
    Task UploadBulk(List<PharmacyDbInputModel> pharmacies);
    Task<List<PharmacyCheckModel>> GetAllCheck();
    Task<List<PharmacyCheckErpModel>> GetAllCheckErp();
    Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId);
    Task BulkUpdateData(List<PharmacyDbUpdateModel> list);

}