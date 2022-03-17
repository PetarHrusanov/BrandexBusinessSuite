namespace BrandexSalesAdapter.ExcelLogic.Services.Pharmacies
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Models.Pharmacies;

    public interface IPharmaciesService
    {
        Task<string> CreatePharmacy(PharmacyInputModel pharmacyInputModel);

        Task<List<PharmacyCheckModel>> GetPharmaciesCheck();

        Task<bool> CheckPharmacyByDistributor(string input, string Distributor);

        Task<int> PharmacyIdByDistributor(string input, string Distributor);

        Task<ICollection<PharmacyDistributorCheck>> PharmacyIdsByDistributorForCheck(string input);

        Task<string> NameById(string input, string distributor);

        Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime date, int? regionId);

    }
}
