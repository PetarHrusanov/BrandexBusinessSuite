namespace SpravkiFirstDraft.Services.PharmacyChains
{
    using System;
    using System.Threading.Tasks;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;

    public class PharmacyChainsService : IPharmacyChainsService
    {
        SpravkiDbContext db;

        public PharmacyChainsService(SpravkiDbContext db)
        {
            this.db = db;
        }

        public async Task<string> UploadPharmacyChain(string chainName)
        {
            if (chainName != null)
            {
                var chainInput = new PharmacyChain
                {
                    Name = chainName
                };

                await this.db.PharmacyChains.AddAsync(chainInput);
                await this.db.SaveChangesAsync();
                return chainName;

            }
            else
            {
                return "";
            }
        }

       
    }
}
