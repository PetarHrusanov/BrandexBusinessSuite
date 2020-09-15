namespace SpravkiFirstDraft.Services.Pharmacies
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SpravkiFirstDraft.Data;
    using static Common.DataConstants.Ditributors;

    public class PharmaciesService :IPharmaciesService
    {
        SpravkiDbContext db;

        public PharmaciesService(SpravkiDbContext db)
        {
            this.db = db;
        }

        public async Task<bool> CheckPharmacyByDistributor(string input, string Distributor)
        {
            int convertedNumber = int.Parse(input);
            switch (Distributor)
            {
                case Brandex:
                    return await this.db.Pharmacies.Where(c => c.BrandexId == convertedNumber).AnyAsync();
                case Sting:
                    return await this.db.Pharmacies.Where(c => c.StingId == convertedNumber).AnyAsync();
                case Phoenix:
                    return await this.db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).AnyAsync();
                case Pharmnet:
                    return await this.db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).AnyAsync();
                case Sopharma:
                    return await this.db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).AnyAsync();
                default:
                    return false;
            }
        }

        public async Task<int> PharmacyIdByDistributor(string input, string Distributor)
        {
                int convertedNumber = int.Parse(input);
                switch (Distributor)
                {
                    case Brandex:
                        return await this.db.Pharmacies.Where(c => c.BrandexId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Sting:
                        return await this.db.Pharmacies.Where(c => c.StingId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Phoenix:
                        return await this.db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Pharmnet:
                        return await this.db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    case Sopharma:
                        return await this.db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).Select(p => p.Id).FirstOrDefaultAsync();
                    default:
                        return 0;
                };
        }
    }
}
