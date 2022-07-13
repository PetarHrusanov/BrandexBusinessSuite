using BrandexBusinessSuite.SalesAnalysis.Models.Pharmacies;

namespace BrandexBusinessSuite.SalesAnalysis.Services.Pharmacies;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesAnalysis.Models.Pharmacies;

public interface IPharmaciesService
{
    Task UploadBulk(List<PharmacyDbInputModel> pharmacies);
    Task<string> CreatePharmacy(PharmacyDbInputModel pharmacyDbInputModel);

    Task<List<PharmacyCheckModel>> GetPharmaciesCheck();

    Task<string> NameById(string input, string distributor);

    Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime? dateBegin, DateTime? dateEnd, int? regionId);

    Task Update(List<PharmacyDbInputModel> pharmacies);
        
    // Task<bool> CheckPharmacyByDistributor(string input, string Distributor);

    // Task<int> PharmacyIdByDistributor(string input, string Distributor);

    // Task<ICollection<PharmacyDistributorCheck>> PharmacyIdsByDistributorForCheck(string input);

}