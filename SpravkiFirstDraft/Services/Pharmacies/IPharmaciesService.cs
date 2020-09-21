namespace SpravkiFirstDraft.Services.Pharmacies
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SpravkiFirstDraft.Models.Pharmacies;

    public interface IPharmaciesService
    {
        Task<string> CreatePharmacy(PharmacyInputModel pharmacyInputModel);

        Task<bool> CheckPharmacyByDistributor(string input, string Distributor);

        Task<int> PharmacyIdByDistributor(string input, string Distributor);

        Task<string> NameById(string input, string distributor);

        Task<List<PharmacyExcelModel>> GetPharmaciesExcelModel(DateTime date, int? regionId);
    }
}
