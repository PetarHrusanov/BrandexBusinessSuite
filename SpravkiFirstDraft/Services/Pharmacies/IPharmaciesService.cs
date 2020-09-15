namespace SpravkiFirstDraft.Services.Pharmacies
{
    using System;
    using System.Threading.Tasks;

    public interface IPharmaciesService
    {
        Task<bool> CheckPharmacyByDistributor(string input, string Distributor);

        Task<int> PharmacyIdByDistributor(string input, string Distributor);

        Task<string> NameById(string input, string distributor);
    }
}
