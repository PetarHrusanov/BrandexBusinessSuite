namespace SpravkiFirstDraft.Services.Pharmacies
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SpravkiFirstDraft.Data;
    using SpravkiFirstDraft.Data.Models;
    using SpravkiFirstDraft.Models.Pharmacies;
    using static Common.DataConstants.Ditributors;

    public class PharmaciesService :IPharmaciesService
    {
        SpravkiDbContext db;

        public PharmaciesService(SpravkiDbContext db)
        {
            this.db = db;
        }

        public async Task<string> CreatePharmacy(PharmacyInputModel pharmacyInputModel)
        {
            if(pharmacyInputModel.BrandexId!=0 &&
                pharmacyInputModel.Name!=null &&
                pharmacyInputModel.PharmacyChainId!= 0 &&
                pharmacyInputModel.RegionId!= 0 &&
                pharmacyInputModel.CityId!= 0 &&
                pharmacyInputModel.CompanyId !=0)
            {
                var pharmacyModel = new Pharmacy
                {
                    BrandexId = pharmacyInputModel.BrandexId,
                    Name = pharmacyInputModel.Name,
                    Address = pharmacyInputModel.Address,
                    PharmacyChainId = pharmacyInputModel.PharmacyChainId,
                    Active = pharmacyInputModel.Active,
                    CityId = pharmacyInputModel.CityId,
                    SopharmaId = pharmacyInputModel.SopharmaId,
                    StingId = pharmacyInputModel.StingId,
                    PhoenixId = pharmacyInputModel.PhoenixId,
                    PharmnetId = pharmacyInputModel.PharmnetId,
                    CompanyId = pharmacyInputModel.CompanyId,
                    PharmacyClass = pharmacyInputModel.PharmacyClass,
                    RegionId = pharmacyInputModel.RegionId

                };

                await this.db.Pharmacies.AddAsync(pharmacyModel);
                await this.db.SaveChangesAsync();
                return pharmacyModel.Name;
            }
            else
            {
                return "";
            }
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

        public async Task<string> NameById(string input, string distributor)
        {
            int convertedNumber;
            bool success = int.TryParse(input, out convertedNumber);

            switch (distributor)
            {
                case Brandex:
                    return await this.db.Pharmacies.Where(c => c.BrandexId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Sting:
                    return await this.db.Pharmacies.Where(c => c.StingId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Phoenix:
                    return await this.db.Pharmacies.Where(c => c.PhoenixId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Pharmnet:
                    return await this.db.Pharmacies.Where(c => c.PharmnetId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                case Sopharma:
                    return await this.db.Pharmacies.Where(c => c.SopharmaId == convertedNumber).Select(p => p.Name).FirstOrDefaultAsync();
                default:
                    return "";
            };
        }
    }
}
