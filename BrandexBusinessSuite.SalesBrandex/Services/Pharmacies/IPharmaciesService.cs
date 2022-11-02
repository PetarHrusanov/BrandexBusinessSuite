namespace BrandexBusinessSuite.SalesBrandex.Services.Pharmacies;

using System.Collections.Generic;
using System.Threading.Tasks;

using BrandexBusinessSuite.SalesBrandex.Models.Pharmacies;

public interface IPharmaciesService
{
    Task UploadBulk(List<PharmacyDbInputModel> pharmacies);
    Task<List<PharmacyCheckModel>> GetPharmaciesCheck();

    // Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId);
    //
    // Task Update(List<PharmacyDbInputModel> pharmacies);

}