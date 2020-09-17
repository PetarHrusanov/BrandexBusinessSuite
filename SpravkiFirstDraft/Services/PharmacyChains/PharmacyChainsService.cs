namespace SpravkiFirstDraft.Services.PharmacyChains
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> CheckPharmacyChainByName(string pharmacyChainName)
        {
            return await db.PharmacyChains.Where(x => x.Name.ToLower().TrimEnd() == pharmacyChainName.ToLower().TrimEnd())
                                    .Select(x => x.Id).AnyAsync();
        }

        public async Task<int> IdByName(string pharmacyChainName)
        {
            return await db.PharmacyChains.Where(x => x.Name.ToLower().TrimEnd() == pharmacyChainName.ToLower().TrimEnd())
                                    .Select(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
