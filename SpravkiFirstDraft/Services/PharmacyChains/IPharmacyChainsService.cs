namespace SpravkiFirstDraft.Services.PharmacyChains
{
    using System;
    using System.Threading.Tasks;

    public interface IPharmacyChainsService
    {
        Task<string> UploadPharmacyChain(string chainName);
    }
}
